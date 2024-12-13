namespace ContinentDemo.WebApi.Logic
{
    using Interfaces;
    using Location;
    using Responses;
    using Newtonsoft.Json;

    public class LocationLogic: ILocationLogic
    {
        private readonly INetworkRequestHandler? _handler;
        private readonly ICacheStorage _cacheStorage;
        private readonly ILogger<LocationLogic> _logger;
        private readonly int _cachingOperationTimeOut;
        private readonly bool _extendedLog;

        public LocationLogic(INetworkRequestHandler? handler, ICacheStorage cacheStorage, ILogger<LocationLogic> logger)
        {
            _handler = handler;
            _cacheStorage = cacheStorage;
            _logger = logger;
            _cachingOperationTimeOut = ConfigAppSettings.CachingOperationTimeOut;
            _extendedLog = ConfigAppSettings.ExtendedLogEnabled;
        }

        public async Task<Location?> GetLocationFromRequestAsync(string iata)
        {
            Location? location = null;
            var response = await _handler?.MakeGetRequestAsync(iata)!;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);
                
                if (airportResponse != null)
                {
                    location = new Location()
                    {
                        Latitude = airportResponse.location!.lat,
                        Longitude = airportResponse.location!.lon
                    };
                }

                return location;
            }
            
            return location;
        }

        public async Task<(double?, string)> GetDistanceAsync(string iata1, string iata2)
        {
            var text = string.Empty;
            var locationTask1 = GetLocationAsync(iata1);
            var locationTask2 = GetLocationAsync(iata2);
            
            await Task.WhenAll(locationTask1, locationTask2);

            var distance = locationTask1.Result.HasValue && locationTask2.Result.HasValue
                ? Distance.GetDistanceInMiles(locationTask1.Result.Value, locationTask2.Result!.Value)
                : (double?)null;

            if (!locationTask1.Result.HasValue | !locationTask2.Result.HasValue)
            {
                text = $"No Location found for: {(!locationTask1.Result.HasValue ? iata1 : string.Empty)} " +
                       $"{(!locationTask2.Result.HasValue ? iata2 : string.Empty)}";
                _logger.Log(LogLevel.Warning, text);
            }

            return (distance, text);
        }

        public async Task<Location?> GetLocationFromCacheAsync(string iata)
        {
            var locationTask = _cacheStorage.GetLocationFromCacheAsync(iata);
            var timeOutTask = Task.Delay(_cachingOperationTimeOut);

            if (await Task.WhenAny(locationTask, timeOutTask) != timeOutTask)
            {
                var location = await locationTask;

                if (_extendedLog)
                {
                    _logger.Log(LogLevel.Information,
                        $"{(location == null ?  "Cannot get": "Get")} from cache location: {iata} - {location.ToString()}");
                }

                return location;
            }
            else
            {
                _logger.Log(LogLevel.Error, "Getting from cache Task timeout...");
                return null;
            }
        }

        public async Task StoreLocationToCacheAsync(string iata, Location location)
        {
            var locationTask = _cacheStorage.StoreLocationToCacheAsync(iata, location);
            var timeOutTask = Task.Delay(_cachingOperationTimeOut);

            if (await Task.WhenAny(locationTask, timeOutTask) != timeOutTask)
            {
                var added = await locationTask;

                if (_extendedLog)
                {
                    _logger.Log((added ? LogLevel.Information: LogLevel.Warning),
                        $"{(added ? "Added " : "Cannot Add")} to cache : {iata} - {location.ToString()}");
                }
            }
            else
            {
                _logger.Log(LogLevel.Error, "Store to cache Task timeout...");
            }
        }

        public async Task<Location?> GetLocationAsync(string iata)
        {
            var locationFromCache = await GetLocationFromCacheAsync(iata);
            var locationFromRequest = locationFromCache ?? await GetLocationFromRequestAsync(iata);
            
            if (locationFromCache == null && locationFromRequest != null)
                await StoreLocationToCacheAsync(iata, (Location)locationFromRequest);
           
            return locationFromCache ?? (locationFromRequest ?? null);
        }
    }
}

namespace ContinentDemo.WebApi.Logic
{
    using Interfaces;
    using Location;
    using Responses;
    using Newtonsoft.Json;

    public class LocationLogic: ILocationLogic
    {
        //public static string KeySeparator { get; } = "#";
        private readonly INetworkRequestHandler? _handler;
        private readonly ICacheStorage _cacheStorage;
        private readonly ILogger<LocationLogic> _logger;
        private readonly int _cachingOperationTimeOut;

        public LocationLogic(INetworkRequestHandler? handler, ICacheStorage cacheStorage, ILogger<LocationLogic> logger)
        {
            _handler = handler;
            _cacheStorage = cacheStorage;
            _logger = logger;
            _cachingOperationTimeOut = ConfigAppSettings.CachingOperationTimeOut;
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

        /*
        public async Task<double?> GetDistanceFromCacheAsync(string iata1, string iata2)
        {
            //var distance12 = await _cacheStorage.GetFromCacheAsync($"{iata1}{KeySeparator}{iata2}");
            //var distance21 = await _cacheStorage.GetFromCacheAsync($"{iata2}{KeySeparator}{iata1}");
            //return distance12 ?? (distance21 ?? -1);

            var pairArray = SortIataPair(iata1, iata2);

            //var distance = await _cacheStorage.GetFromCacheAsync($"{pairArray[0]}{KeySeparator}{pairArray[1]}");
            //return distance;

            var getFromCacheTask = _cacheStorage.GetDistanceFromCacheAsync($"{pairArray[0]}{KeySeparator}{pairArray[1]}");

            try
            {
                var distance = await getFromCacheTask.WaitAsync(TimeSpan.FromMilliseconds(_cachingOperationTimeOut));
                return distance;
            }
            catch (TimeoutException)
            {
                _logger.Log(LogLevel.Error, "Getting from cache Task timeout...");
                return null;
            }

        }
        */


        public async Task<double?> GetDistanceAsync(string iata1, string iata2)
        {
            var locationTask1 = GetLocationAsync(iata1);
            var locationTask2 = GetLocationAsync(iata2);
            
            await Task.WhenAll(locationTask1, locationTask2);

            var distance = locationTask1.Result.HasValue && locationTask2.Result.HasValue
                ? Distance.GetDistanceInMiles(locationTask1.Result.Value, locationTask2.Result!.Value)
                : (double?)null;

            return distance;
            //location1 ??= 
        }

        /*
        public async Task StoreDistanceToCacheAsync(string iata1, string iata2, double distance)
        {
            //await _cacheStorage.StoreToCacheAsync($"{iata1}{KeySeparator}{iata2}", distance);

            var pairArray = SortIataPair(iata1, iata2);

            //await _cacheStorage.StoreToCacheAsync($"{pairArray[0]}{KeySeparator}{pairArray[1]}", distance);

            var storeToCacheTask = _cacheStorage.StoreDistanceToCacheAsync($"{pairArray[0]}{KeySeparator}{pairArray[1]}", distance);

            try
            {
                await storeToCacheTask.WaitAsync(TimeSpan.FromMilliseconds(_cachingOperationTimeOut));
            }
            catch (TimeoutException)
            {
                _logger.Log(LogLevel.Error, "Store to cache Task timeout...");
            }
        }
        */


        public async Task<Location?> GetLocationFromCacheAsync(string iata)
        {
            var locationTask = _cacheStorage.GetLocationFromCacheAsync(iata);
            var timeOutTask = Task.Delay(_cachingOperationTimeOut);

            if (await Task.WhenAny(locationTask, timeOutTask) != timeOutTask)
            {
                return await locationTask;
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
                await locationTask;
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

        private string[] SortIataPair(string iata1, string iata2)
        {
            var pairArray = new[] { iata1, iata2 };
            Array.Sort(pairArray);
            return pairArray;
        }
    }
}

namespace ContinentDemo.WebApi.Services
{
    using Interfaces;
    using Location;
    using System.Diagnostics;

    public class DistanceService: IDistanceService
    {
        private readonly ILocationLogic _locationLogic;
        //private readonly ILogger _logger;
        private readonly ILogger<DistanceService> _logger;

        public DistanceService(ILocationLogic locationLogic, ILogger<DistanceService> logger)
        {
            _locationLogic = locationLogic;
            _logger = logger;
        }

        public async Task<double> GetDistanceBetweenIataAsync(string iata1, string iata2)
        {
            //var location = await _locationLogic.GetLocationFromRequestAsync(iata1);
            //return 0.111;
            var result = await GetDistanceFromIatasAsync(iata1, iata2);
            return result;
        }

        private async Task<double> GetDistanceFromIatasAsync(string iata1, string iata2)
        {
            //---------------------------------------------
            //var stopwatch = new Stopwatch(); stopwatch.Start();
            //var cachedDistance = await _locationLogic.GetDistanceFromCacheAsync(iata1, iata2);

            //if (cachedDistance.HasValue)
            //{
            //    stopwatch.Stop(); _logger.Log(LogLevel.Information, $"Time for query - {stopwatch.ElapsedMilliseconds} ms");

            //    return (double)cachedDistance;
            //}

            //var locationTask1 = _locationLogic.GetLocationFromRequestAsync(iata1);
            //var locationTask2 = _locationLogic.GetLocationFromRequestAsync(iata2);

            //await Task.WhenAll(locationTask1, locationTask2);

            //var distance = locationTask1.Result!.HasValue && locationTask2.Result!.HasValue
            //    ? Distance.GetDistanceInMiles(locationTask1.Result!.Value, locationTask2.Result!.Value)
            //    : -1;

            //if (distance > 0)
            //    await _locationLogic.StoreDistanceToCacheAsync(iata1, iata2, distance);
            //else 
            //    _logger.Log(LogLevel.Warning, $"Distance result for {iata1}-{iata2} is {distance}");

            //stopwatch.Stop(); _logger.Log(LogLevel.Information,$"Time for query - {stopwatch.ElapsedMilliseconds} ms");

            //return distance;
            //---------------------------------------------

            var stopwatch = new Stopwatch(); stopwatch.Start();

            var distance = await _locationLogic.GetDistanceAsync(iata1, iata2);

            if (distance == null)
                _logger.Log(LogLevel.Warning, $"Distance result for {iata1}-{iata2} is {distance}");

            stopwatch.Stop(); _logger.Log(LogLevel.Information, $"Time for query - {stopwatch.ElapsedMilliseconds} ms");

            return distance ?? -1;

            //---------------------------------------------
            //var locationTask3 = _locationLogic.GetLocationFromRequestAsync(iata1);
            //var locationTask4 = _locationLogic.GetLocationFromRequestAsync(iata1);
            //var locationTask5 = _locationLogic.GetLocationFromRequestAsync(iata1);

            //var location1 = await locationTask1;
            //var location2 = await locationTask2;
            //var location3 = await locationTask3;
            //var location4 = await locationTask4;
            //var location5 = await locationTask5;

            //stopwatch.Stop();
            //Console.WriteLine($"Time from start - {stopwatch.ElapsedMilliseconds} ms");

            //return location1.HasValue && location2.HasValue
            //    ? Distance.GetDistanceInMiles(locationTask1.Result!.Value, locationTask2.Result!.Value)
            //    : -1;
        }
    }
}

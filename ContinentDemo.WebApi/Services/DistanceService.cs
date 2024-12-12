namespace ContinentDemo.WebApi.Services
{
    using Interfaces;
    using System.Diagnostics;

    public class DistanceService: IDistanceService
    {
        private readonly ILocationLogic _locationLogic;
        private readonly ILogger<DistanceService> _logger;
        private readonly bool _useTimer;

        public DistanceService(ILocationLogic locationLogic, ILogger<DistanceService> logger)
        {
            _locationLogic = locationLogic;
            _logger = logger;
            _useTimer = ConfigAppSettings.UseTimer;
        }

        public async Task<(double, string)> GetDistanceBetweenIataAsync(string iata1, string iata2)
        {
            var stopwatch = new Stopwatch();
            if (_useTimer) stopwatch.Start();

            var (distance, text) = await _locationLogic.GetDistanceAsync(iata1, iata2);

            if (distance is null or <= 0)
                _logger.Log(LogLevel.Warning, $"Distance result for {iata1}-{iata2} is {(distance == null ? "null" : distance)}");
            
            if (_useTimer)
                stopwatch.Stop(); _logger.Log(LogLevel.Information, $"Time spent - {stopwatch.ElapsedMilliseconds} ms");

            return (distance ?? -1, text);
        }
    }
}

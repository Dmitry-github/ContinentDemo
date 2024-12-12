namespace ContinentDemo.WebApi.Logic
{
    using Interfaces;
    using Polly;
    using Polly.Retry;
    using System.Net;
    using System.Net.Http.Headers;
    
    public class NetworkRequestHandler: INetworkRequestHandler
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly string? _host;
        private readonly string? _requestUri;
        private readonly ILogger<NetworkRequestHandler> _logger;
        
        public NetworkRequestHandler(string? host, string? reqestUri, ILogger<NetworkRequestHandler> logger)
        {
            _host = host;
            _requestUri = reqestUri;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_host!);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            _logger = logger;

            //retry on HttpRequestException, NetWorkRetryCount retries with exponential backoff
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r =>
                    !r.IsSuccessStatusCode && r.StatusCode != HttpStatusCode.NotFound && r.StatusCode != HttpStatusCode.BadRequest)
                .Or<HttpRequestException>()

                .WaitAndRetryAsync(ConfigAppSettings.NetWorkRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.Log(LogLevel.Warning,
                            $"Request failed with {outcome.Exception?.Message}.Waiting {timespan} before next retry.Retry attempt {retryAttempt}.");
                    });
        }

        public async Task<HttpResponseMessage> MakeGetRequestAsync(string param)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.Log(LogLevel.Information, $"Making request to {_host}{_requestUri}");
                
                var response = await _httpClient.GetAsync($"{_requestUri}/{param}");
                if (!response.IsSuccessStatusCode)
                {
                    var text = $"Request to '{_requestUri}/{param}' failed with status code '{response.StatusCode}'";
                    _logger.Log(LogLevel.Warning, text);

                    if (response.StatusCode != HttpStatusCode.NotFound && response.StatusCode != HttpStatusCode.BadRequest)
                        throw new HttpRequestException(text);
                }
                return response;
            });
        }
    }
}

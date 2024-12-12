namespace ContinentDemo.WebApi.Interfaces
{
    public interface INetworkRequestHandler
    {
        public Task<HttpResponseMessage> MakeGetRequestAsync(string requestUri);
    }
}
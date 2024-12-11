namespace ContinentDemo.WebApi.Interfaces
{
    public interface IDistanceService
    {
        Task<double> GetDistanceBetweenIataAsync(string iata1, string iata2);
    }
}
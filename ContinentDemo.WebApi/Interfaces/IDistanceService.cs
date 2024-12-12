namespace ContinentDemo.WebApi.Interfaces
{
    public interface IDistanceService
    {
        Task<(double, string)> GetDistanceBetweenIataAsync(string iata1, string iata2);
    }
}
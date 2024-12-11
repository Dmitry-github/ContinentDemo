namespace ContinentDemo.Tests
{
    using WebApi.Location;
    using System.Diagnostics;

    public class DistanceUnitTests
    {
        Location _vkoLocation, _pskLocation;

        [Fact]
        public void TestDistanceInKm()
        {
            SetupLocations();

            var distance = Distance.GetDistance(_pskLocation, _vkoLocation);

            Debug.WriteLine($"distance in km = {distance}");

            Assert.NotEqual(0, distance);
            Assert.True(distance > 594);
            Assert.True(distance < 595);
        }

        [Fact]
        public void TestDistanceInMiles()
        {
            SetupLocations();

            var distance = Distance.GetDistanceInMiles(_pskLocation, _vkoLocation);

            Debug.WriteLine($"distance in miles = {distance}");

            Assert.NotEqual(0, distance);
            Assert.True(distance > 320);
            Assert.True(distance < 321);
        }


        void SetupLocations()
        {
            _vkoLocation = new Location
            {
                Longitude = 37.292098,
                Latitude = 55.60315
            };

            _pskLocation = new Location
            {
                Longitude = 28.397663,
                Latitude = 57.790968
            };
        }
    }
}
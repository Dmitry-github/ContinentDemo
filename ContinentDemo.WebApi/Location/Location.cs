namespace ContinentDemo.WebApi.Location
{
    using System.Text.Json;

    public struct Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Location? FromString(string? text)
        {
            return string.IsNullOrEmpty(text) ? null : JsonSerializer.Deserialize<Location>(text);
        }
    }
}

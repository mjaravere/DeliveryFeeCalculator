namespace DeliveryFeeCalculator.Models.Classes
{
    public class WeatherObservation
    {
        public int Id { get; set; }
        public required string StationName { get; set; }
        public required string WMOCode { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public required string WeatherPhenomenon { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow.AddHours(2);
    }
}
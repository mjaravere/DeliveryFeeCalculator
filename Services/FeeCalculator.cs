using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Models.Enums;
using DeliveryFeeCalculator.Models.Classes;
using DeliveryFeeCalculator.Models.Exceptions;

namespace DeliveryFeeCalculator.Services
{
    public class FeeCalculator
    {
        private readonly WeatherObservationsRepo _repo;
        private readonly IConfiguration _configuration;

        public FeeCalculator(WeatherObservationsRepo repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }
        
        public async Task<decimal> CalculateFee(City city, Vehicle vehicle)
        {
            var basePrice = GetRegionalBaseFee(city, vehicle);
            var latestWeather = await GetLatestWeatherData(city);

            var extraFees = CalculateExtraFees(latestWeather, vehicle);
            
            return basePrice + extraFees;
        }

        private decimal GetRegionalBaseFee(City city, Vehicle vehicle)
        {
            var cityFees = _configuration.GetSection($"RegionalBaseFees:{city}").Get<Dictionary<string, decimal>>();
            if (cityFees == null)
            {
                throw new ArgumentException($"No base fees configured for city: {city}");
            }

            if (!cityFees.TryGetValue(vehicle.ToString(), out decimal fee))
            {
                throw new ArgumentException($"No base fee configured for vehicle type {vehicle} in city {city}");
            }

            return fee;
        }

        private async Task<WeatherObservation> GetLatestWeatherData(City city)
        {
            var stationName = GetWeatherStationForCity(city);
            var latestObservation = await _repo.GetLatestByStationName(stationName);
            
            if (latestObservation == null)
            {
                throw new InvalidOperationException($"No weather data available for {city}");
            }

            return latestObservation;
        }

        private string GetWeatherStationForCity(City city)
        {
            return city switch
            {
                City.Tallinn => "Tallinn-Harku",
                City.Tartu => "Tartu-Tõravere",
                City.Pärnu => "Pärnu",
                _ => throw new ArgumentException($"Unknown city: {city}")
            };
        }

        private decimal CalculateExtraFees(WeatherObservation weather, Vehicle vehicle)
        {
            var temperatureFee = CalculateTemperatureFee(weather.Temperature, vehicle);
            var windSpeedFee = CalculateWindSpeedFee(weather.WindSpeed, vehicle);
            var weatherPhenomenonFee = CalculateWeatherPhenomenonFee(weather.WeatherPhenomenon, vehicle);

            return temperatureFee + windSpeedFee + weatherPhenomenonFee;
        }

        private decimal CalculateTemperatureFee(double temperature, Vehicle vehicle)
        {
            if (vehicle != Vehicle.Scooter && vehicle != Vehicle.Bike)
            {
                return 0m;
            }

            if (temperature < -10)
            {
                return 1.0m;
            }
            else if (temperature >= -10 && temperature <= 0)
            {
                return 0.5m;
            }

            return 0m;
        }

        private decimal CalculateWindSpeedFee(double windSpeed, Vehicle vehicle)
        {
            if (vehicle != Vehicle.Bike)
            {
                return 0m;
            }

            if (windSpeed > 20)
            {
                throw new ForbiddenVehicleException("Usage of selected vehicle type is forbidden");
            }
            else if (windSpeed >= 10 && windSpeed <= 20)
            {
                return 0.5m;
            }

            return 0m;
        }

        private decimal CalculateWeatherPhenomenonFee(string phenomenon, Vehicle vehicle)
        {
            if (vehicle != Vehicle.Scooter && vehicle != Vehicle.Bike)
            {
                return 0m;
            }

            if (phenomenon.Contains("glaze") || phenomenon.Contains("hail") || phenomenon.Contains("thunder"))
            {
                throw new ForbiddenVehicleException("Usage of selected vehicle type is forbidden");
            }

            if (phenomenon.Contains("snow") || phenomenon.Contains("sleet"))
            {
                return 1.0m;
            }

            if (phenomenon.Contains("rain"))
            {
                return 0.5m;
            }

            return 0m;
        }
    }
}
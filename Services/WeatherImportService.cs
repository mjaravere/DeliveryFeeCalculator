using System.Xml.Linq;
using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Models.Classes;

namespace DeliveryFeeCalculator.Services;

public class WeatherImportService
{
    private readonly WeatherObservationsRepo _repo;
    private readonly HttpClient _httpClient;
    private readonly string[] _targetStations = { "Tallinn-Harku", "Tartu-Tõravere", "Pärnu" };
    private const string WEATHER_URL = "https://www.ilmateenistus.ee/ilma_andmed/xml/observations.php";

    public WeatherImportService(WeatherObservationsRepo repo, HttpClient httpClient)
    {
        _repo = repo;
        _httpClient = httpClient;
    }

    public async Task ImportWeatherData()
    {
        try
        {
            var xmlContent = await _httpClient.GetStringAsync(WEATHER_URL);
            var doc = XDocument.Parse(xmlContent);
            var stations = doc.Descendants("station");

            foreach (var station in stations)
            {
                var stationName = station.Element("name")?.Value;
                
                if (_targetStations.Contains(stationName))
                {
                    var weatherObservation = new WeatherObservation
                    {
                        StationName = stationName,
                        WMOCode = station.Element("wmocode")?.Value ?? "N/A",
                        Temperature = double.Parse(station.Element("airtemperature")?.Value ?? "0"),
                        WindSpeed = double.Parse(station.Element("windspeed")?.Value ?? "0"),
                        WeatherPhenomenon = station.Element("phenomenon")?.Value ?? "N/A",
                        Timestamp = DateTime.UtcNow
                    };

                    await _repo.SaveObservationToDb(weatherObservation);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
} 
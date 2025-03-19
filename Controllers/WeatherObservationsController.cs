using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryFeeCalculator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherObservationsController(WeatherObservationsRepo repo, WeatherImportService weatherImportService) : ControllerBase()
    {
        private readonly WeatherObservationsRepo repo = repo;
        private readonly WeatherImportService _weatherImportService = weatherImportService;

        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var result = await repo.GetAllWeatherObservations();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeatherObservation(int id){
            var weatherObservation = await repo.GetWeatherObservationById(id);
            if (weatherObservation is null){
                return NotFound($"Ilmavaatlust ID-ga {id} ei leitud.");
            }
            return Ok(weatherObservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherObservation(int id){
            bool result = await repo.DeleteWeatherObservation(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportWeatherData()
        {
            try
            {
                await _weatherImportService.ImportWeatherData();
                return Ok("Ilmaandmete import õnnestus.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ilmaandmete importimisel tekkis viga: {ex.Message}");
            }
        }
    }
}
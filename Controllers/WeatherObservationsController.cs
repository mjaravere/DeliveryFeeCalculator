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

        /// <summary>
        /// Gets all weather observations
        /// </summary>
        /// <returns>List of all weather observations</returns>
        /// <response code="200">Returns the list of weather observations</response>
        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var result = await repo.GetAllWeatherObservations();
            return Ok(result);
        }

        /// <summary>
        /// Gets a weather observation by ID
        /// </summary>
        /// <param name="id">ID of the weather observation</param>
        /// <returns>Weather observation</returns>
        /// <response code="200">Returns the weather observation</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeatherObservation(int id){
            var weatherObservation = await repo.GetWeatherObservationById(id);
            if (weatherObservation is null){
                return NotFound($"Weather observation with ID {id} not found.");
            }
            return Ok(weatherObservation);
        }

        /// <summary>
        /// Deletes a weather observation by ID
        /// </summary>
        /// <param name="id">ID of the weather observation</param>
        /// <returns>No content</returns>
        /// <response code="204">Weather observation deleted successfully</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherObservation(int id){
            bool result = await repo.DeleteWeatherObservation(id);
            return result ? NoContent() : NotFound();
        }
        
        /// <summary>
        /// Imports weather data
        /// </summary>
        /// <returns>Status of the import operation</returns>
        /// <response code="200">Weather data successfully imported</response>
        /// <response code="500">If import operation fails</response>
        [HttpPost("import")]
        public async Task<IActionResult> ImportWeatherData()
        {
            try
            {
                await _weatherImportService.ImportWeatherData();
                return Ok("Weather data import successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while importing weather data: {ex.Message}");
            }
        }
    }
}
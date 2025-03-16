using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryFeeCalculator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherObservationsController(WeatherObservationsRepo repo) : ControllerBase()
    {
        private readonly WeatherObservationsRepo repo = repo;

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

        [HttpPost]
        public async Task<IActionResult> SaveWeatherObservation([FromBody] WeatherObservation weatherObservation){
            var weatherOservationExists = await repo.WeatherObservationExists(weatherObservation.Id);
            if (weatherOservationExists){
                return Conflict($"Ilmavaatlust ID-ga {weatherObservation.Id} on juba olemas.");
            }
            
            var result = await repo.SaveObservationToDb(weatherObservation);
            return CreatedAtAction(nameof(SaveWeatherObservation), new { weatherObservation.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeatherObservation(int id, [FromBody] WeatherObservation weatherObservation){
            bool result = await repo.UpdateWeatherObservation(id, weatherObservation);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherObservation(int id){
            bool result = await repo.DeleteWeatherObservation(id);
            return result ? NoContent() : NotFound();
        }
    }
}
using DeliveryFeeCalculator.Models.Enums;
using DeliveryFeeCalculator.Models.Exceptions;
using DeliveryFeeCalculator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryFeeCalculator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryFeeController : ControllerBase
    {
        private readonly FeeCalculator _calculator;

        public DeliveryFeeController(FeeCalculator calculator)
        {
            _calculator = calculator;
        }
        
        /// <summary>
        /// Calculates the delivery fee based on city and vehicle type
        /// </summary>
        /// <param name="city">Delivery destination city (0 - Tallinn, 1 - Tartu, 2 - Pärnu)</param>
        /// <param name="vehicle">Vehicle used for delivery (0 - Car, 1 - Scooter, 2 - Bike)</param>
        /// <returns>Delivery fee in euros</returns>
        /// <response code="200">Returns the calculated delivery fee</response>
        /// <response code="400">If the vehicle is forbidden in current weather conditions</response>
        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateFee([FromQuery] City city, [FromQuery] Vehicle vehicle)
        {
            try
            {
                var fee = await _calculator.CalculateFee(city, vehicle);
                return Ok(new { DeliveryFee = fee });
            }
            catch (ForbiddenVehicleException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
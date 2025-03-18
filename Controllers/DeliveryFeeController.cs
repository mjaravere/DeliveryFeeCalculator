using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
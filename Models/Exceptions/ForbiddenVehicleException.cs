namespace DeliveryFeeCalculator.Models.Exceptions
{
    public class ForbiddenVehicleException : Exception
    {
        public ForbiddenVehicleException(string message) : base(message)
        {
        }
    }
} 
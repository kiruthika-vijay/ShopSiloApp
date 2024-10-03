namespace ShopSiloAppFSD.Server.Services
{
    public class OrderService : IOrderService
    {
        private static readonly Random random = new Random();

        // Method to generate a tracking number in the format TRK123456789
        public string GenerateOrderTrackingNumber()
        {
            // Prefix
            string prefix = "TRK";

            // Random number generator for the numeric part
            string numericPart = GenerateRandomNumber(9);

            // Construct the tracking number
            string trackingNumber = $"{prefix}{numericPart}";

            return trackingNumber;
        }

        // Helper method to generate a random number with a specified length
        private string GenerateRandomNumber(int length)
        {
            string number = string.Empty;

            // Generate a random number of the specified length
            for (int i = 0; i < length; i++)
            {
                number += random.Next(0, 10);  // Generates a number between 0 and 9
            }

            return number;
        }
    }

}

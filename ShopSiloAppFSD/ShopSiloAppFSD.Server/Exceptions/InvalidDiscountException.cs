namespace ShopSiloAppFSD.Server.Exceptions
{
    public class InvalidDiscountException : Exception
    {
        public InvalidDiscountException(string message) : base(message) { }
        public InvalidDiscountException(string message, Exception innerException) : base(message, innerException) { }
    }
}

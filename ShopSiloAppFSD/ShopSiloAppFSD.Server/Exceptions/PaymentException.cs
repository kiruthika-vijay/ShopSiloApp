namespace ShopSiloAppFSD.Exceptions
{
    public class PaymentException : ApplicationException
    {
        public PaymentException(string message) : base(message) { }
        public PaymentException(string message, Exception innerException) : base(message, innerException) { }
    }
}

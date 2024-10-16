namespace ShopSiloAppFSD.Exceptions
{
    public class RazorpayException : Exception
    {
        public RazorpayException(string message) : base(message) { }
        public RazorpayException(string message, Exception innerException) : base(message, innerException) { }
    }
}

namespace ShopSiloAppFSD.Exceptions
{
    public class ShippingException : ApplicationException
    {
        public ShippingException(string message) : base(message) { }
        public ShippingException(string message, Exception innerException) : base(message, innerException) { }
    }
}

namespace ShopSiloAppFSD.Exceptions
{
    public class ProductManagementException : ApplicationException
    {
        public ProductManagementException(string message) : base(message) { }
        public ProductManagementException(string message, Exception innerException) : base(message, innerException) { }
    }
}

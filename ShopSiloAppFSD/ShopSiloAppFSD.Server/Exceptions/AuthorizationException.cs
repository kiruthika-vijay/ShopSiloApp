namespace ShopSiloAppFSD.Exceptions
{
    public class AuthorizationException : ApplicationException
    {
        public AuthorizationException(string message) : base(message) { }
        public AuthorizationException(string message, Exception innerException) : base(message, innerException) { }
    }
}

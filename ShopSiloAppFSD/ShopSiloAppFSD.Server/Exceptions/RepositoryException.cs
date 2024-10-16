namespace ShopSiloAppFSD.Exceptions
{
    public class RepositoryException : ApplicationException
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}

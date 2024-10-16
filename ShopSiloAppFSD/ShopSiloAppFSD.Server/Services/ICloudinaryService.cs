namespace ShopSiloAppFSD.Server.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImage(string publicId);
        string GetPublicIdFromUrl(string imageUrl);
    }
}

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Server.Services;
using Microsoft.AspNetCore.Http;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var cloudName = config["Cloudinary:CloudName"];
        var apiKey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];
        var acc = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(acc);
    }

    // Upload image to Cloudinary
    public async Task<string> UploadImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new ArgumentException("Image file is required.");

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(image.FileName, image.OpenReadStream()),
            Transformation = new Transformation().Height(500).Width(500).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Error uploading image to Cloudinary");

        return uploadResult.SecureUrl.AbsoluteUri;
    }

    // Delete image from Cloudinary
    public async Task<bool> DeleteImage(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }

    // Extract public ID from Cloudinary URL
    public string GetPublicIdFromUrl(string url)
    {
        // Assuming Cloudinary URL structure, extract public ID
        var uri = new Uri(url);
        var segments = uri.Segments;
        var publicId = segments.Last().Split('.')[0]; // Get the last segment and split to remove the file extension
        return publicId;
    }
}

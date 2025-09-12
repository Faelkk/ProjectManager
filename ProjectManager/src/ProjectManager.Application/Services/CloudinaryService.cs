using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ProjectManager.Application.DTOS;
using ProjectManager.Application.Interfaces;


namespace ProjectManager.Application.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> options)
    {
        var settings = options.Value;
        var account = new Account(settings.CloudName, settings.ApiKey, settings.Secret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<CloudinaryUploadResult> UploadImage(string fileName, Stream fileStream)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return new CloudinaryUploadResult
            {
                Url = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };
        }
        else
        {
            throw new Exception($"Falha no upload do arquivo: {uploadResult.Error?.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

        return deleteResult.Result == "ok";
    }
}
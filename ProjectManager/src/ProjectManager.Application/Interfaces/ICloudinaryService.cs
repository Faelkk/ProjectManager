using ProjectManager.Application.DTOS;

namespace ProjectManager.Application.Interfaces;

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadImage(string fileName, Stream fileStream);

    Task<bool> DeleteFileAsync(string publicId);
}
using ProjectManager.Application.Common.Interfaces;

namespace ProjectManager.Application.FileValidator
{
    public static class FileValidator
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; 

        public static void ValidateImage(IAppFile file)
        {
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ApplicationException($"Formato de arquivo '{extension}' não permitido. Use jpg, jpeg, png ou gif.");

            if (file.Length > MaxFileSizeBytes)
                throw new ApplicationException($"O arquivo '{file.Name}' excede o tamanho máximo permitido de 5 MB.");
        }
    }
}
using DonaRogApp.Domain.Storage;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Application.Storage
{
    /// <summary>
    /// Local filesystem implementation of file storage
    /// </summary>
    public class LocalFileStorageService : IFileStorageService, ITransientDependency
    {
        private readonly FileStorageOptions _options;

        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
            
            // Ensure base directory exists
            if (!string.IsNullOrWhiteSpace(_options.BasePath))
            {
                Directory.CreateDirectory(_options.BasePath);
            }
        }

        public async Task<string> SaveFileAsync(Stream stream, string fileName, string? subfolder = null)
        {
            Check.NotNull(stream, nameof(stream));
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));

            // Generate unique filename to avoid collisions
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            // Build path
            var relativePath = subfolder != null 
                ? Path.Combine(subfolder, uniqueFileName)
                : uniqueFileName;
            
            var fullPath = Path.Combine(_options.BasePath, relativePath);
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save file
            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            // Return relative path for storage in database
            return relativePath.Replace("\\", "/"); // Normalize path separators
        }

        public async Task<Stream> GetFileAsync(string storagePath)
        {
            Check.NotNullOrWhiteSpace(storagePath, nameof(storagePath));

            var fullPath = Path.Combine(_options.BasePath, storagePath);
            
            if (!File.Exists(fullPath))
            {
                throw new BusinessException("DonaRog:FileNotFound")
                    .WithData("storagePath", storagePath);
            }

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        public Task DeleteFileAsync(string storagePath)
        {
            Check.NotNullOrWhiteSpace(storagePath, nameof(storagePath));

            var fullPath = Path.Combine(_options.BasePath, storagePath);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<bool> FileExistsAsync(string storagePath)
        {
            Check.NotNullOrWhiteSpace(storagePath, nameof(storagePath));

            var fullPath = Path.Combine(_options.BasePath, storagePath);
            return Task.FromResult(File.Exists(fullPath));
        }

        public Task<string?> GetPresignedUrlAsync(string storagePath, int expirationMinutes = 60)
        {
            // Local storage doesn't support presigned URLs
            // Return null - the API will serve the file directly
            return Task.FromResult<string?>(null);
        }
    }
}

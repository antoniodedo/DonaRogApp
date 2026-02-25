using System.IO;
using System.Threading.Tasks;

namespace DonaRogApp.Domain.Storage
{
    /// <summary>
    /// Service for storing and retrieving files
    /// Abstraction over different storage providers (local, Azure Blob, S3, etc)
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Save a file to storage
        /// </summary>
        /// <param name="stream">File content stream</param>
        /// <param name="fileName">Original filename</param>
        /// <param name="subfolder">Optional subfolder (e.g., "2024/01" for organizing by date)</param>
        /// <returns>Storage path/key for the saved file</returns>
        Task<string> SaveFileAsync(Stream stream, string fileName, string? subfolder = null);
        
        /// <summary>
        /// Get a file from storage
        /// </summary>
        /// <param name="storagePath">Storage path/key</param>
        /// <returns>File content stream</returns>
        Task<Stream> GetFileAsync(string storagePath);
        
        /// <summary>
        /// Delete a file from storage
        /// </summary>
        /// <param name="storagePath">Storage path/key</param>
        Task DeleteFileAsync(string storagePath);
        
        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="storagePath">Storage path/key</param>
        Task<bool> FileExistsAsync(string storagePath);
        
        /// <summary>
        /// Get a presigned/temporary URL for accessing the file (for cloud storage)
        /// </summary>
        /// <param name="storagePath">Storage path/key</param>
        /// <param name="expirationMinutes">URL validity duration in minutes</param>
        /// <returns>URL to access the file</returns>
        Task<string?> GetPresignedUrlAsync(string storagePath, int expirationMinutes = 60);
    }
}

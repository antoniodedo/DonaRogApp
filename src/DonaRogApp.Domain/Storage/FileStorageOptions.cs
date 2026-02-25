namespace DonaRogApp.Domain.Storage
{
    public class FileStorageOptions
    {
        /// <summary>
        /// Storage provider type: Local, AzureBlob, S3, etc.
        /// </summary>
        public FileStorageProvider Provider { get; set; } = FileStorageProvider.Local;
        
        /// <summary>
        /// For Local: Base directory path (e.g., "D:/DonaRogApp/Files" or "/var/donarog/files")
        /// For Azure: Container name
        /// For S3: Bucket name
        /// </summary>
        public string BasePath { get; set; } = "Files/Donations";
        
        /// <summary>
        /// Maximum file size in MB
        /// </summary>
        public int MaxFileSizeMB { get; set; } = 10;
        
        /// <summary>
        /// Allowed file extensions (comma separated)
        /// </summary>
        public string AllowedExtensions { get; set; } = ".jpg,.jpeg,.png,.pdf,.gif";
        
        /// <summary>
        /// Azure Blob Storage connection string (if Provider = AzureBlob)
        /// </summary>
        public string? AzureConnectionString { get; set; }
        
        /// <summary>
        /// AWS S3 settings (if Provider = S3)
        /// </summary>
        public S3Settings? S3 { get; set; }
        
        /// <summary>
        /// Generate URL with expiration for downloads
        /// </summary>
        public bool UsePresignedUrls { get; set; } = false;
        
        /// <summary>
        /// Presigned URL expiration in minutes
        /// </summary>
        public int PresignedUrlExpirationMinutes { get; set; } = 60;
    }
    
    public enum FileStorageProvider
    {
        /// <summary>
        /// Store files on local server filesystem
        /// </summary>
        Local = 0,
        
        /// <summary>
        /// Store files on Azure Blob Storage
        /// </summary>
        AzureBlob = 1,
        
        /// <summary>
        /// Store files on AWS S3
        /// </summary>
        S3 = 2
    }
    
    public class S3Settings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = "eu-south-1";
        public string BucketName { get; set; } = string.Empty;
    }
}

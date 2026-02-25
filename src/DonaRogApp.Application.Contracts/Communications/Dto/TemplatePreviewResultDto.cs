namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// Result of template preview
    /// </summary>
    public class TemplatePreviewResultDto
    {
        /// <summary>
        /// Preview content (HTML or PDF base64)
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// Content type
        /// </summary>
        public PreviewOutputFormat Format { get; set; }

        /// <summary>
        /// File size in bytes (for PDF)
        /// </summary>
        public long? FileSizeBytes { get; set; }

        /// <summary>
        /// MIME type
        /// </summary>
        public string MimeType => Format == PreviewOutputFormat.Pdf ? "application/pdf" : "text/html";
    }
}

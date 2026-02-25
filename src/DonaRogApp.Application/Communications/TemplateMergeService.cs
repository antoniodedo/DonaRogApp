using DinkToPdf;
using DinkToPdf.Contracts;
using DonaRogApp.Enums.Communications;
using Ganss.Xss;
using Mammoth;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DonaRogApp.Application.Communications
{
    /// <summary>
    /// Service for template conversion and PDF generation
    /// Handles: HTML→PDF, DOCX→HTML conversion, PDF merging
    /// </summary>
    public class TemplateMergeService : ITransientDependency
    {
        private readonly IConverter _pdfConverter;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly ILogger<TemplateMergeService> _logger;

        public TemplateMergeService(
            IConverter pdfConverter,
            ILogger<TemplateMergeService> logger)
        {
            _pdfConverter = pdfConverter;
            _logger = logger;
            
            // Configure HTML sanitizer for security
            _htmlSanitizer = new HtmlSanitizer();
            _htmlSanitizer.AllowedTags.Add("style");
            _htmlSanitizer.AllowedTags.Add("link");
            _htmlSanitizer.AllowedAttributes.Add("style");
            _htmlSanitizer.AllowedAttributes.Add("class");
        }

        // ======================================================================
        // DOCX → HTML CONVERSION
        // ======================================================================

        /// <summary>
        /// Convert DOCX file to HTML using Mammoth
        /// </summary>
        public Task<string> ConvertDocxToHtmlAsync(Stream docxStream)
        {
            try
            {
                var converter = new DocumentConverter();
                var result = converter.ConvertToHtml(docxStream);
                
                if (result.Warnings.Any())
                {
                    _logger.LogWarning("DOCX conversion warnings: {Warnings}", 
                        string.Join("; ", result.Warnings));
                }
                
                return Task.FromResult(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert DOCX to HTML");
                throw new BusinessException("DonaRog:DocxConversionFailed")
                    .WithData("error", ex.Message);
            }
        }

        /// <summary>
        /// Convert DOCX file path to HTML
        /// </summary>
        public async Task<string> ConvertDocxToHtmlAsync(string docxFilePath)
        {
            if (!File.Exists(docxFilePath))
            {
                throw new BusinessException("DonaRog:DocxFileNotFound")
                    .WithData("path", docxFilePath);
            }

            using var stream = File.OpenRead(docxFilePath);
            return await ConvertDocxToHtmlAsync(stream);
        }

        /// <summary>
        /// Convert Word merge fields to standard placeholder format
        /// Word merge fields: «FieldName» or {{ MERGEFIELD FieldName }}
        /// Convert to: {{FieldName}}
        /// </summary>
        public string ConvertWordMergeFields(string html)
        {
            // Handle Word merge fields: «FieldName»
            html = System.Text.RegularExpressions.Regex.Replace(
                html, 
                @"«(\w+)»", 
                "{{$1}}");
            
            // Handle Word MERGEFIELD syntax: {{ MERGEFIELD FieldName }}
            html = System.Text.RegularExpressions.Regex.Replace(
                html,
                @"\{\{\s*MERGEFIELD\s+(\w+)\s*\}\}",
                "{{$1}}",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            return html;
        }

        // ======================================================================
        // HTML → PDF CONVERSION
        // ======================================================================

        /// <summary>
        /// Convert HTML to PDF using DinkToPdf (wkhtmltopdf wrapper)
        /// </summary>
        public byte[] ConvertHtmlToPdf(
            string html,
            string? title = null,
            PaperKind paperSize = PaperKind.A4,
            bool landscape = false,
            int marginTop = 10,
            int marginBottom = 10,
            int marginLeft = 10,
            int marginRight = 10)
        {
            try
            {
                // Wrap HTML in full document if needed
                var fullHtml = WrapHtmlDocument(html, title);
                
                // Sanitize HTML for security
                var sanitizedHtml = _htmlSanitizer.Sanitize(fullHtml);
                
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = landscape ? Orientation.Landscape : Orientation.Portrait,
                    PaperSize = paperSize,
                    Margins = new MarginSettings 
                    { 
                        Top = marginTop, 
                        Bottom = marginBottom, 
                        Left = marginLeft, 
                        Right = marginRight 
                    },
                    DocumentTitle = title ?? "Document"
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = sanitizedHtml,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontSize = 9, Right = "Generated on [date]", Line = true }
                };

                var document = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                return _pdfConverter.Convert(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert HTML to PDF");
                throw new BusinessException("DonaRog:HtmlToPdfConversionFailed")
                    .WithData("error", ex.Message);
            }
        }

        /// <summary>
        /// Convert HTML file to PDF
        /// </summary>
        public byte[] ConvertHtmlFileToPdf(string htmlFilePath, string? title = null)
        {
            if (!File.Exists(htmlFilePath))
            {
                throw new BusinessException("DonaRog:HtmlFileNotFound")
                    .WithData("path", htmlFilePath);
            }

            var html = File.ReadAllText(htmlFilePath);
            return ConvertHtmlToPdf(html, title);
        }

        /// <summary>
        /// Save PDF bytes to file
        /// </summary>
        public async Task SavePdfAsync(byte[] pdfBytes, string outputPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(outputPath, pdfBytes);
                _logger.LogInformation("PDF saved to {Path}, size: {Size} bytes", outputPath, pdfBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save PDF to {Path}", outputPath);
                throw new BusinessException("DonaRog:PdfSaveFailed")
                    .WithData("path", outputPath)
                    .WithData("error", ex.Message);
            }
        }

        // ======================================================================
        // PDF MERGING
        // ======================================================================

        /// <summary>
        /// Merge multiple PDFs into single PDF document
        /// Used for batch printing (combine all thank you letters)
        /// </summary>
        public byte[] MergePdfs(List<byte[]> pdfBytesList)
        {
            if (pdfBytesList == null || !pdfBytesList.Any())
            {
                throw new BusinessException("DonaRog:NoPdfsToMerge");
            }

            try
            {
                using var outputDocument = new PdfDocument();
                
                foreach (var pdfBytes in pdfBytesList)
                {
                    using var ms = new MemoryStream(pdfBytes);
                    using var inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                    
                    // Copy all pages from input to output
                    for (int i = 0; i < inputDocument.PageCount; i++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[i]);
                    }
                }
                
                // Save to memory stream
                using var outputStream = new MemoryStream();
                outputDocument.Save(outputStream, false);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to merge PDFs");
                throw new BusinessException("DonaRog:PdfMergeFailed")
                    .WithData("pdfCount", pdfBytesList.Count)
                    .WithData("error", ex.Message);
            }
        }

        /// <summary>
        /// Merge PDF files from disk
        /// </summary>
        public byte[] MergePdfFiles(List<string> pdfFilePaths)
        {
            var pdfBytesList = new List<byte[]>();
            
            foreach (var path in pdfFilePaths)
            {
                if (!File.Exists(path))
                {
                    throw new BusinessException("DonaRog:PdfFileNotFound")
                        .WithData("path", path);
                }
                
                pdfBytesList.Add(File.ReadAllBytes(path));
            }
            
            return MergePdfs(pdfBytesList);
        }

        // ======================================================================
        // HELPER METHODS
        // ======================================================================

        /// <summary>
        /// Wrap HTML fragment in full HTML document with basic styling
        /// </summary>
        private string WrapHtmlDocument(string htmlContent, string? title = null)
        {
            // Check if already a full HTML document
            if (htmlContent.Contains("<html", StringComparison.OrdinalIgnoreCase) &&
                htmlContent.Contains("</html>", StringComparison.OrdinalIgnoreCase))
            {
                return htmlContent;
            }

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine($"<title>{title ?? "Document"}</title>");
            sb.AppendLine("<meta charset=\"utf-8\">");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; font-size: 12pt; line-height: 1.6; }");
            sb.AppendLine("h1, h2, h3 { color: #333; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("table, th, td { border: 1px solid #ddd; padding: 8px; }");
            sb.AppendLine("th { background-color: #f2f2f2; text-align: left; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine(htmlContent);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }

        /// <summary>
        /// Validate that template content is safe and not empty
        /// </summary>
        public void ValidateTemplateContent(string content, TemplateType templateType)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new BusinessException("DonaRog:TemplateContentEmpty");
            }

            if (templateType == TemplateType.Html)
            {
                // Basic HTML validation
                if (!content.Contains("<") && !content.Contains(">"))
                {
                    throw new BusinessException("DonaRog:InvalidHtmlTemplate")
                        .WithData("reason", "No HTML tags found");
                }
            }
        }

        /// <summary>
        /// Estimate PDF size based on HTML content length
        /// Rough estimate: 1 KB HTML ≈ 3-5 KB PDF
        /// </summary>
        public long EstimatePdfSize(string htmlContent)
        {
            var htmlSizeBytes = Encoding.UTF8.GetByteCount(htmlContent);
            return htmlSizeBytes * 4; // 4x multiplier as rough estimate
        }
    }
}

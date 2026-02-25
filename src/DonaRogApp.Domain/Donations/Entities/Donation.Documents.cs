using DonaRogApp.Enums.Donations;
using System;
using System.Linq;
using Volo.Abp;

namespace DonaRogApp.Domain.Donations.Entities
{
    public partial class Donation
    {
        /// <summary>
        /// Add a document with physical file to the donation
        /// </summary>
        public DonationDocument AddDocument(
            string fileName,
            string fileExtension,
            string mimeType,
            long fileSizeBytes,
            string storagePath,
            DonationDocumentType documentType,
            bool isFromExternalFlow = false)
        {
            Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            Check.NotNullOrWhiteSpace(fileExtension, nameof(fileExtension));
            Check.NotNullOrWhiteSpace(mimeType, nameof(mimeType));
            Check.Positive(fileSizeBytes, nameof(fileSizeBytes));
            Check.NotNullOrWhiteSpace(storagePath, nameof(storagePath));

            var document = new DonationDocument(
                Guid.NewGuid(),
                Id,
                fileName,
                fileExtension,
                mimeType,
                fileSizeBytes,
                storagePath,
                documentType,
                isFromExternalFlow
            );

            Documents.Add(document);
            
            return document;
        }

        /// <summary>
        /// Add a text-only document to the donation (e.g., bank transfer description from external import)
        /// </summary>
        public DonationDocument AddTextDocument(
            string textContent,
            DonationDocumentType documentType,
            bool isFromExternalFlow = true)
        {
            Check.NotNullOrWhiteSpace(textContent, nameof(textContent));

            var document = DonationDocument.CreateTextDocument(
                Guid.NewGuid(),
                Id,
                textContent,
                documentType,
                isFromExternalFlow
            );

            Documents.Add(document);
            
            return document;
        }

        /// <summary>
        /// Remove a document from the donation
        /// </summary>
        public void RemoveDocument(Guid documentId)
        {
            var document = Documents.FirstOrDefault(d => d.Id == documentId);
            if (document == null)
            {
                throw new BusinessException("DonaRog:DonationDocumentNotFound")
                    .WithData("donationId", Id)
                    .WithData("documentId", documentId);
            }

            Documents.Remove(document);
        }

        /// <summary>
        /// Check if donation has documents
        /// </summary>
        public bool HasDocuments() => Documents.Any();

        /// <summary>
        /// Get total number of documents
        /// </summary>
        public int GetDocumentCount() => Documents.Count;
    }
}

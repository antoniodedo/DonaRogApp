using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Application.LetterTemplates
{
    /// <summary>
    /// Service for rendering letter templates with tag substitution
    /// </summary>
    public class TemplateRenderer : ITransientDependency
    {
        private static readonly Regex TagPattern = new(@"\{\{([^\}]+)\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Render template content with provided tag values
        /// </summary>
        public string Render(string content, Dictionary<string, string> tagValues)
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;

            if (tagValues == null || tagValues.Count == 0)
                return content;

            // Replace all {{TagName}} with corresponding values
            return TagPattern.Replace(content, match =>
            {
                var tagName = match.Groups[1].Value.Trim();
                
                // Try to get value from dictionary (case-insensitive)
                foreach (var kvp in tagValues)
                {
                    if (string.Equals(kvp.Key, tagName, StringComparison.OrdinalIgnoreCase))
                    {
                        return kvp.Value ?? string.Empty;
                    }
                }

                // If tag not found, keep the placeholder or return empty
                return match.Value; // Keep {{TagName}} if not found
            });
        }

        /// <summary>
        /// Get all tag names from template content
        /// </summary>
        public List<string> ExtractTags(string content)
        {
            var tags = new List<string>();
            
            if (string.IsNullOrWhiteSpace(content))
                return tags;

            var matches = TagPattern.Matches(content);
            foreach (Match match in matches)
            {
                var tagName = match.Groups[1].Value.Trim();
                if (!tags.Any(t => string.Equals(t, tagName, StringComparison.OrdinalIgnoreCase)))
                {
                    tags.Add(tagName);
                }
            }

            return tags;
        }

        /// <summary>
        /// Build tag values dictionary from donor and donation data
        /// </summary>
        public Dictionary<string, string> BuildDonorTagValues(
            Domain.Donors.Entities.Donor donor,
            decimal? donationAmount = null,
            DateTime? donationDate = null,
            string? donationReference = null,
            Domain.Projects.Entities.Project? project = null,
            Domain.Recurrences.Entities.Recurrence? recurrence = null)
        {
            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Donor data
            if (donor != null)
            {
                tags["DonorTitle"] = ""; // TODO: Get from Title navigation
                tags["DonorFirstName"] = donor.FirstName ?? "";
                tags["DonorMiddleName"] = donor.MiddleName ?? "";
                tags["DonorLastName"] = donor.LastName ?? "";
                tags["DonorName"] = donor.GetFullName();
                tags["DonorCompanyName"] = donor.CompanyName ?? "";
                
                // Primary email (first verified or first one)
                var primaryEmail = donor.Emails?.FirstOrDefault(e => e.IsVerified) ?? donor.Emails?.FirstOrDefault();
                tags["DonorEmail"] = primaryEmail?.EmailAddress ?? "";
                
                // Primary address (first verified or first one)
                var primaryAddress = donor.Addresses?.FirstOrDefault(a => a.IsVerified) ?? donor.Addresses?.FirstOrDefault();
                if (primaryAddress != null)
                {
                    tags["DonorStreet"] = primaryAddress.Street ?? "";
                    tags["DonorCity"] = primaryAddress.City ?? "";
                    tags["DonorProvince"] = primaryAddress.Province ?? "";
                    tags["DonorPostalCode"] = primaryAddress.PostalCode ?? "";
                    tags["DonorCountry"] = primaryAddress.Country ?? "";
                    tags["DonorFullAddress"] = $"{primaryAddress.Street}, {primaryAddress.PostalCode} {primaryAddress.City} ({primaryAddress.Province})";
                }
            }

            // Donation data
            if (donationAmount.HasValue)
            {
                tags["DonationAmount"] = donationAmount.Value.ToString("N2");
                tags["DonationAmountInWords"] = ConvertAmountToWords(donationAmount.Value);
            }

            if (donationDate.HasValue)
            {
                tags["DonationDate"] = donationDate.Value.ToString("dd/MM/yyyy");
            }

            if (!string.IsNullOrEmpty(donationReference))
            {
                tags["DonationReference"] = donationReference;
            }

            // Project data
            if (project != null)
            {
                tags["ProjectName"] = project.Name;
                tags["ProjectCode"] = project.Code;
                tags["ProjectDescription"] = project.Description ?? "";
                tags["ProjectGoal"] = project.TargetAmount?.ToString("N2") ?? "";
            }

            // Recurrence data
            if (recurrence != null)
            {
                tags["RecurrenceName"] = recurrence.Name;
                tags["RecurrenceDate"] = recurrence.GetRecurrenceDateForYear(DateTime.Now.Year)?.ToString("dd/MM/yyyy") ?? "";
                tags["RecurrenceYear"] = DateTime.Now.Year.ToString();
            }

            // Current date
            tags["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
            tags["CurrentYear"] = DateTime.Now.Year.ToString();
            
            // Organization (placeholder - can be configured)
            tags["OrganizationName"] = "DonaRog"; // TODO: Get from settings

            return tags;
        }

        /// <summary>
        /// Convert numeric amount to words (Italian)
        /// Basic implementation - can be enhanced
        /// </summary>
        private string ConvertAmountToWords(decimal amount)
        {
            // Simplified implementation for MVP
            // TODO: Implement full Italian number-to-words conversion
            var intPart = (int)Math.Floor(amount);
            var decPart = (int)Math.Round((amount - intPart) * 100);

            if (decPart > 0)
                return $"{intPart},{decPart:00} euro";
            else
                return $"{intPart} euro";
        }
    }
}

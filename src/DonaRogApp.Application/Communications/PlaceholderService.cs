using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Domain.Donors.Entities;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;

namespace DonaRogApp.Application.Communications
{
    /// <summary>
    /// Service for managing template placeholders and data merging
    /// </summary>
    public class PlaceholderService : ITransientDependency
    {
        // ======================================================================
        // PLACEHOLDER CONSTANTS
        // ======================================================================
        
        // Donor placeholders
        public const string DonorTitle = "DonorTitle";
        public const string DonorFirstName = "DonorFirstName";
        public const string DonorLastName = "DonorLastName";
        public const string DonorFullName = "DonorFullName";
        public const string DonorCompanyName = "DonorCompanyName";
        
        // Address placeholders
        public const string DonorStreet = "DonorStreet";
        public const string DonorCity = "DonorCity";
        public const string DonorProvince = "DonorProvince";
        public const string DonorRegion = "DonorRegion";
        public const string DonorPostalCode = "DonorPostalCode";
        public const string DonorCountry = "DonorCountry";
        public const string DonorFullAddress = "DonorFullAddress";
        
        // Donation placeholders
        public const string DonationReference = "DonationReference";
        public const string DonationAmount = "DonationAmount";
        public const string DonationAmountFormatted = "DonationAmountFormatted";
        public const string DonationAmountInWords = "DonationAmountInWords";
        public const string DonationDate = "DonationDate";
        public const string DonationDateFormatted = "DonationDateFormatted";
        public const string DonationChannel = "DonationChannel";
        
        // Project placeholders
        public const string ProjectName = "ProjectName";
        public const string ProjectDescription = "ProjectDescription";
        public const string ProjectNames = "ProjectNames";
        
        // System/Date placeholders
        public const string CurrentDate = "CurrentDate";
        public const string CurrentDateFormatted = "CurrentDateFormatted";
        public const string CurrentYear = "CurrentYear";
        
        // Donor statistics
        public const string DonorTotalDonated = "DonorTotalDonated";
        public const string DonorDonationCount = "DonorDonationCount";
        public const string DonorFirstDonationDate = "DonorFirstDonationDate";
        
        // Organization placeholders (to be configured)
        public const string OrgName = "OrgName";
        public const string OrgAddress = "OrgAddress";
        public const string OrgTaxCode = "OrgTaxCode";
        public const string OrgPhone = "OrgPhone";
        public const string OrgEmail = "OrgEmail";
        public const string OrgWebsite = "OrgWebsite";

        // ======================================================================
        // PUBLIC METHODS
        // ======================================================================

        /// <summary>
        /// Get all available placeholders with descriptions
        /// </summary>
        public Dictionary<string, string> GetAllPlaceholders()
        {
            return new Dictionary<string, string>
            {
                // Donor
                { DonorTitle, "Titolo donatore (Sig., Dott., Prof., etc.)" },
                { DonorFirstName, "Nome donatore" },
                { DonorLastName, "Cognome donatore" },
                { DonorFullName, "Nome completo donatore (Nome Cognome)" },
                { DonorCompanyName, "Ragione sociale (per organizzazioni)" },
                
                // Address
                { DonorStreet, "Via/Indirizzo donatore" },
                { DonorCity, "Città donatore" },
                { DonorProvince, "Provincia donatore" },
                { DonorRegion, "Regione donatore" },
                { DonorPostalCode, "CAP donatore" },
                { DonorCountry, "Nazione donatore" },
                { DonorFullAddress, "Indirizzo completo (via, CAP, città)" },
                
                // Donation
                { DonationReference, "Riferimento donazione (es. DON-2026-00123)" },
                { DonationAmount, "Importo donazione (es. 150.50)" },
                { DonationAmountFormatted, "Importo formattato (es. €150,50)" },
                { DonationAmountInWords, "Importo in lettere (es. centocinquanta euro e cinquanta centesimi)" },
                { DonationDate, "Data donazione (formato ISO)" },
                { DonationDateFormatted, "Data donazione formattata (es. 22 febbraio 2026)" },
                { DonationChannel, "Canale donazione (Bonifico, PayPal, etc.)" },
                
                // Project
                { ProjectName, "Nome progetto principale" },
                { ProjectDescription, "Descrizione progetto" },
                { ProjectNames, "Lista nomi progetti (se più allocazioni)" },
                
                // System
                { CurrentDate, "Data corrente (formato ISO)" },
                { CurrentDateFormatted, "Data corrente formattata" },
                { CurrentYear, "Anno corrente" },
                
                // Statistics
                { DonorTotalDonated, "Totale donato dal donatore (lifetime)" },
                { DonorDonationCount, "Numero donazioni del donatore" },
                { DonorFirstDonationDate, "Data prima donazione" },
                
                // Organization
                { OrgName, "Nome organizzazione" },
                { OrgAddress, "Indirizzo organizzazione" },
                { OrgTaxCode, "Codice fiscale organizzazione" },
                { OrgPhone, "Telefono organizzazione" },
                { OrgEmail, "Email organizzazione" },
                { OrgWebsite, "Sito web organizzazione" }
            };
        }

        /// <summary>
        /// Build merge data dictionary from donor and donation entities
        /// </summary>
        public Dictionary<string, string> BuildMergeData(
            Donor donor,
            Donation donation,
            Dictionary<string, string>? organizationData = null)
        {
            var data = new Dictionary<string, string>();
            
            // Donor data
            if (donor.SubjectType == Enums.Donors.SubjectType.Individual)
            {
                data[DonorTitle] = donor.Title?.Abbreviation ?? "";
                data[DonorFirstName] = donor.FirstName ?? "";
                data[DonorLastName] = donor.LastName ?? "";
                data[DonorFullName] = $"{donor.FirstName} {donor.LastName}".Trim();
                data[DonorCompanyName] = "";
            }
            else
            {
                data[DonorTitle] = "";
                data[DonorFirstName] = "";
                data[DonorLastName] = "";
                data[DonorFullName] = donor.CompanyName ?? "";
                data[DonorCompanyName] = donor.CompanyName ?? "";
            }
            
            // Address data
            var primaryAddress = donor.Addresses?.FirstOrDefault(a => a.IsDefault && a.EndDate == null);
            if (primaryAddress != null)
            {
                data[DonorStreet] = primaryAddress.Street;
                data[DonorCity] = primaryAddress.City;
                data[DonorProvince] = primaryAddress.Province ?? "";
                data[DonorRegion] = primaryAddress.Region ?? "";
                data[DonorPostalCode] = primaryAddress.PostalCode;
                data[DonorCountry] = primaryAddress.Country;
                data[DonorFullAddress] = $"{primaryAddress.Street}\n{primaryAddress.PostalCode} {primaryAddress.City} ({primaryAddress.Province})";
            }
            else
            {
                data[DonorStreet] = "";
                data[DonorCity] = "";
                data[DonorProvince] = "";
                data[DonorRegion] = "";
                data[DonorPostalCode] = "";
                data[DonorCountry] = "";
                data[DonorFullAddress] = "";
            }
            
            // Donation data
            data[DonationReference] = donation.Reference;
            data[DonationAmount] = donation.TotalAmount.ToString("F2", CultureInfo.InvariantCulture);
            data[DonationAmountFormatted] = donation.TotalAmount.ToString("C2", new CultureInfo("it-IT"));
            data[DonationAmountInWords] = ConvertAmountToWords(donation.TotalAmount);
            data[DonationDate] = donation.DonationDate.ToString("yyyy-MM-dd");
            data[DonationDateFormatted] = donation.DonationDate.ToString("dd MMMM yyyy", new CultureInfo("it-IT"));
            data[DonationChannel] = donation.Channel.ToString();
            
            // Project data (if available)
            if (donation.Projects?.Any() == true)
            {
                var firstProject = donation.Projects.First();
                data[ProjectName] = ""; // Will need to load project entity separately
                data[ProjectDescription] = "";
                data[ProjectNames] = string.Join(", ", donation.Projects.Select(p => p.ProjectId.ToString()));
            }
            else
            {
                data[ProjectName] = "";
                data[ProjectDescription] = "";
                data[ProjectNames] = "";
            }
            
            // System data
            var now = DateTime.Now;
            data[CurrentDate] = now.ToString("yyyy-MM-dd");
            data[CurrentDateFormatted] = now.ToString("dd MMMM yyyy", new CultureInfo("it-IT"));
            data[CurrentYear] = now.Year.ToString();
            
            // Donor statistics
            data[DonorTotalDonated] = donor.TotalDonated.ToString("C2", new CultureInfo("it-IT"));
            data[DonorDonationCount] = donor.DonationCount.ToString();
            data[DonorFirstDonationDate] = donor.FirstDonationDate?.ToString("dd MMMM yyyy", new CultureInfo("it-IT")) ?? "";
            
            // Organization data (from configuration)
            if (organizationData != null)
            {
                foreach (var kvp in organizationData)
                {
                    data[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // Default empty values
                data[OrgName] = "";
                data[OrgAddress] = "";
                data[OrgTaxCode] = "";
                data[OrgPhone] = "";
                data[OrgEmail] = "";
                data[OrgWebsite] = "";
            }
            
            return data;
        }

        /// <summary>
        /// Replace all placeholders in template with actual data
        /// Placeholder format: {{PlaceholderName}}
        /// </summary>
        public string ReplacePlaceholders(string template, Dictionary<string, string> data)
        {
            if (string.IsNullOrWhiteSpace(template))
                return template;
            
            var result = template;
            
            foreach (var kvp in data)
            {
                var placeholder = $"{{{{{kvp.Key}}}}}";
                result = result.Replace(placeholder, kvp.Value ?? "");
            }
            
            return result;
        }

        /// <summary>
        /// Extract all placeholder names from template
        /// Returns list of placeholder names found (without {{ }})
        /// </summary>
        public List<string> ExtractPlaceholders(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                return new List<string>();
            
            var regex = new Regex(@"\{\{(\w+)\}\}");
            var matches = regex.Matches(template);
            
            return matches
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Validate that all placeholders in template are recognized
        /// Returns list of unrecognized placeholders
        /// </summary>
        public List<string> ValidatePlaceholders(string template)
        {
            var foundPlaceholders = ExtractPlaceholders(template);
            var knownPlaceholders = GetAllPlaceholders().Keys.ToList();
            
            return foundPlaceholders
                .Where(p => !knownPlaceholders.Contains(p))
                .ToList();
        }

        // ======================================================================
        // PRIVATE HELPER METHODS
        // ======================================================================

        /// <summary>
        /// Convert decimal amount to Italian words
        /// Example: 150.50 → "centocinquanta euro e cinquanta centesimi"
        /// </summary>
        private string ConvertAmountToWords(decimal amount)
        {
            try
            {
                var euros = (int)Math.Floor(amount);
                var cents = (int)Math.Round((amount - euros) * 100);
                
                var result = euros.ToWords(new CultureInfo("it-IT"));
                
                if (cents > 0)
                {
                    result += $" euro e {cents.ToWords(new CultureInfo("it-IT"))} centesimi";
                }
                else
                {
                    result += " euro";
                }
                
                return result;
            }
            catch
            {
                return amount.ToString("C2", new CultureInfo("it-IT"));
            }
        }
    }
}

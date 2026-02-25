using System.Collections.Generic;
using System.Linq;

namespace DonaRogApp.Application.Contracts.Communications.Dto
{
    /// <summary>
    /// Information about available placeholders
    /// </summary>
    public class PlaceholderInfoDto
    {
        /// <summary>
        /// Placeholder name (without {{ }})
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description in Italian
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Category (Donor, Donation, System, etc.)
        /// </summary>
        public string Category { get; set; } = null!;

        /// <summary>
        /// Example value
        /// </summary>
        public string? ExampleValue { get; set; }
    }

    /// <summary>
    /// List of all available placeholders grouped by category
    /// </summary>
    public class PlaceholderListDto
    {
        public Dictionary<string, List<PlaceholderInfoDto>> PlaceholdersByCategory { get; set; } = new();
        
        public int TotalCount => PlaceholdersByCategory.Values.Sum(list => list.Count);
    }
}

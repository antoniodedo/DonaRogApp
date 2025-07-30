using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class Address : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid DonorId { get; set; }

        public string? RawAddress { get; set; }

        public string? Dug { get; set; }
        public string? Street { get; set; }
        public string? CivicNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }

  
        // Coordinates (optional, for OpenStreetMap integration)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

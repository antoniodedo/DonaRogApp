using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DonaRogApp.Donors.Entities
{
    public class Donor : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        //public string? Title { get; set; }  
        public string? FirstName { get; set; }
        //public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        public string? RawAddress { get; set; }
        public string? RawCap { get; set; }
        public string? RawComune { get; set; }
        //public string? SecondLastName { get; set; }
        //public string? Gender { get; set; }

        //public bool IsOrganization { get; set; } = false;
        //public string? OrganizationName { get; set; }

        //public ContactPreference ContactPreference { get; set; } = ContactPreference.None;
        //public bool WantsThanks { get; set; } = true;
        //public bool PaperDeliveryEnabled { get; set; } = false;
        //public bool EmailDeliveryEnabled { get; set; } = true;

        //public DonorStatus Status { get; set; } = DonorStatus.Active;
        //public int Score { get; set; } = 0;

        //public List<Email> Emails { get; set; } = new();
        //public List<PhoneNumber> PhoneNumbers { get; set; } = new();
        //public List<Address> Addresses { get; set; } = new();
        //public List<Note> Notes { get; set; } = new();
        //public List<DonorRelationship> Relationships { get; set; } = new();
        //public List<DonorTag> Tags { get; set; } = new();
    }
}

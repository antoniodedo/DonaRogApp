using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace DonaRogApp.Domain.Donors.Entities
{
    // ======================================================================
    // DONORINTEREST.CS - Many-to-Many: Donor ↔ Interest
    // ======================================================================
    /// <summary>
    /// Many-to-Many Mapping: Donor ←→ Interest
    /// Represents donor interests in specific topics/themes
    /// Un donatore può avere interesse per più aree tematiche
    /// Include engagement tracking per capire quali temi interessano davvero
    /// </summary>
    public class DonorInterest : Entity, IMultiTenant
    {
        // --------------------------------------------------------------
        // MULTI-TENANCY
        // --------------------------------------------------------------
        /// <summary>
        /// Tenant ID
        /// </summary>
        public Guid? TenantId { get; private set; }

        // --------------------------------------------------------------
        // RELATIONSHIP PROPERTIES
        // --------------------------------------------------------------
        /// <summary>
        /// Donor ID (Foreign Key)
        /// </summary>
        public Guid DonorId { get; private set; }

        /// <summary>
        /// Donor (navigation property)
        /// </summary>
        public virtual Donor? Donor { get; private set; }

        /// <summary>
        /// Interest ID (Foreign Key)
        /// </summary>
        public Guid InterestId { get; private set; }

        /// <summary>
        /// Interest (navigation property)
        /// Interest entity is in Domain/Shared/Entities/
        /// </summary>
        public virtual Interest? Interest { get; private set; }

        // --------------------------------------------------------------
        // TRACKING
        // --------------------------------------------------------------
        /// <summary>
        /// Data in cui il donatore ha manifestato interesse
        /// </summary>
        public DateTime InterestedSince { get; private set; }

        /// <summary>
        /// Data in cui l'interesse è stato rimosso (NULL = ancora interessato)
        /// </summary>
        public DateTime? RemovedAt { get; private set; }

        /// <summary>
        /// Livello di interesse: 1=basso, 5=altissimo
        /// </summary>
        public int InterestLevel { get; private set; }

        /// <summary>
        /// Score di engagement basato su interazioni (0-100)
        /// </summary>
        public int EngagementScore { get; private set; }

        /// <summary>
        /// Numero di donazioni effettuate su questo tema
        /// </summary>
        public int DonationCountOnTopic { get; private set; }

        /// <summary>
        /// Importo totale donato su questo tema
        /// </summary>
        public decimal TotalDonatedOnTopic { get; private set; }

        /// <summary>
        /// Numero di comunicazioni su questo tema ricevute dal donatore
        /// </summary>
        public int CommunicationsReceivedOnTopic { get; private set; }

        /// <summary>
        /// Numero di comunicazioni su questo tema aperte/cliccate
        /// </summary>
        public int CommunicationsEngagedOnTopic { get; private set; }

        /// <summary>
        /// Come è stato rilevato l'interesse (es: "FormRegistrazione", "DonationPattern")
        /// </summary>
        public string? DiscoveryMethod { get; private set; }

        /// <summary>
        /// Note aggiuntive
        /// </summary>
        public string? Notes { get; private set; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------
        /// <summary>
        /// Protected constructor for EF Core
        /// Use factory method Create() to instantiate
        /// </summary>
        protected DonorInterest()
        {
        }

        // --------------------------------------------------------------
        // FACTORY METHODS
        // --------------------------------------------------------------
        /// <summary>
        /// Creates new DonorInterest mapping (manuale)
        /// </summary>
        internal static DonorInterest CreateManual(
            Guid donorId,
            Guid interestId,
            Guid? tenantId,
            int interestLevel = 3,
            string? notes = null)
        {
            if (donorId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(donorId));
            if (interestId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(interestId));
            Check.Range(interestLevel, nameof(interestLevel), 1, 5);

            return new DonorInterest
            {
                DonorId = donorId,
                InterestId = interestId,
                TenantId = tenantId,
                InterestedSince = DateTime.UtcNow,
                RemovedAt = null,
                InterestLevel = interestLevel,
                EngagementScore = 0,
                DonationCountOnTopic = 0,
                TotalDonatedOnTopic = 0,
                CommunicationsReceivedOnTopic = 0,
                CommunicationsEngagedOnTopic = 0,
                DiscoveryMethod = "ManualAssignment",
                Notes = notes
            };
        }

        /// <summary>
        /// Creates new DonorInterest mapping (automatico)
        /// </summary>
        internal static DonorInterest CreateAutomatic(
            Guid donorId,
            Guid interestId,
            Guid? tenantId,
            string discoveryMethod,
            int interestLevel = 3,
            string? notes = null)
        {
            if (donorId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(donorId));
            if (interestId == Guid.Empty) throw new ArgumentException("Value cannot be empty", nameof(interestId));
            Check.NotNullOrWhiteSpace(discoveryMethod, nameof(discoveryMethod));
            Check.Range(interestLevel, nameof(interestLevel), 1, 5);

            return new DonorInterest
            {
                DonorId = donorId,
                InterestId = interestId,
                TenantId = tenantId,
                InterestedSince = DateTime.UtcNow,
                RemovedAt = null,
                InterestLevel = interestLevel,
                EngagementScore = 0,
                DonationCountOnTopic = 0,
                TotalDonatedOnTopic = 0,
                CommunicationsReceivedOnTopic = 0,
                CommunicationsEngagedOnTopic = 0,
                DiscoveryMethod = discoveryMethod,
                Notes = notes
            };
        }

        // --------------------------------------------------------------
        // METHODS
        // --------------------------------------------------------------
        /// <summary>
        /// Registra una donazione su questo tema
        /// </summary>
        public void RecordDonationOnTopic(decimal amount)
        {
            Check.Positive(amount, nameof(amount));

            DonationCountOnTopic++;
            TotalDonatedOnTopic += amount;
            RecalculateEngagementScore();
        }

        /// <summary>
        /// Registra una comunicazione ricevuta su questo tema
        /// </summary>
        public void RecordCommunicationReceived()
        {
            CommunicationsReceivedOnTopic++;
        }

        /// <summary>
        /// Registra engagement (apertura/click) su comunicazione del tema
        /// </summary>
        public void RecordCommunicationEngagement()
        {
            CommunicationsEngagedOnTopic++;
            RecalculateEngagementScore();
        }

        /// <summary>
        /// Ricalcola l'engagement score basato su metriche
        /// Formula: (Donazioni*20) + (Engagement*10) + (InterestLevel*5)
        /// Scala 0-100
        /// </summary>
        private void RecalculateEngagementScore()
        {
            var donationScore = Math.Min(DonationCountOnTopic * 20, 40);
            var engagementScore = Math.Min(CommunicationsEngagedOnTopic * 10, 40);
            var interestLevelScore = InterestLevel * 5;

            EngagementScore = Math.Min((int)(donationScore + engagementScore + interestLevelScore), 100);
        }

        /// <summary>
        /// Aggiorna il livello di interesse
        /// </summary>
        public void UpdateInterestLevel(int level)
        {
            Check.Range(level, nameof(level), 1, 5);
            InterestLevel = level;
            RecalculateEngagementScore();
        }

        /// <summary>
        /// Rimuove l'interesse del donatore su questo tema
        /// </summary>
        public void Remove()
        {
            if (RemovedAt.HasValue)
                return;

            RemovedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Controlla se l'interesse è attualmente attivo
        /// </summary>
        public bool IsActive => !RemovedAt.HasValue;

        /// <summary>
        /// Calcola il tasso di engagement su questo tema (%)
        /// </summary>
        public decimal GetEngagementRate()
        {
            if (CommunicationsReceivedOnTopic == 0)
                return 0;

            return (decimal)CommunicationsEngagedOnTopic / CommunicationsReceivedOnTopic * 100;
        }

        /// <summary>
        /// Calcola il valore medio della donazione su questo tema
        /// </summary>
        public decimal GetAverageDonationAmount()
        {
            if (DonationCountOnTopic == 0)
                return 0;

            return TotalDonatedOnTopic / DonationCountOnTopic;
        }

        /// <summary>
        /// Calcola i giorni da quando il donatore è interessato
        /// </summary>
        public int GetDaysInterested()
        {
            var endDate = RemovedAt ?? DateTime.UtcNow;
            return (int)(endDate - InterestedSince).TotalDays;
        }

        /// <summary>
        /// Aggiorna le note
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        // --------------------------------------------------------------
        // ENTITY KEY
        // --------------------------------------------------------------
        public override object[] GetKeys()
        {
            return new object[] { DonorId, InterestId };
        }
    }
}

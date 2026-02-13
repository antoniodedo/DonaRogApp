using System;
using System.Linq;
using System.Threading.Tasks;
using DonaRogApp.Domain.BankAccounts.Entities;
using DonaRogApp.Domain.Donations.Entities;
using DonaRogApp.Enums.Donations;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace DonaRogApp.Data;

public class DonationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Donation, Guid> _donationRepository;
    private readonly IRepository<Domain.Donors.Entities.Donor, Guid> _donorRepository;
    private readonly IRepository<BankAccount, Guid> _bankAccountRepository;
    private readonly IRepository<Domain.Campaigns.Entities.Campaign, Guid> _campaignRepository;
    private readonly IRepository<Domain.Projects.Entities.Project, Guid> _projectRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DonationDataSeedContributor(
        IRepository<Donation, Guid> donationRepository,
        IRepository<Domain.Donors.Entities.Donor, Guid> donorRepository,
        IRepository<BankAccount, Guid> bankAccountRepository,
        IRepository<Domain.Campaigns.Entities.Campaign, Guid> campaignRepository,
        IRepository<Domain.Projects.Entities.Project, Guid> projectRepository,
        IGuidGenerator guidGenerator)
    {
        _donationRepository = donationRepository;
        _donorRepository = donorRepository;
        _bankAccountRepository = bankAccountRepository;
        _campaignRepository = campaignRepository;
        _projectRepository = projectRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Skip if donations already exist
        if (await _donationRepository.GetCountAsync() > 0)
        {
            return;
        }

        // Get donors
        var donors = await _donorRepository.GetListAsync();
        if (!donors.Any())
        {
            return; // No donors, skip seed
        }

        // Get or create bank account
        var bankAccount = await _bankAccountRepository.FirstOrDefaultAsync();
        if (bankAccount == null)
        {
            bankAccount = BankAccount.Create(
                _guidGenerator.Create(),
                null,
                "Conto Principale",
                "IT60X0542811101000000123456",
                "Banca Intesa",
                "BCITITMM"
            );
            await _bankAccountRepository.InsertAsync(bankAccount, autoSave: true);
        }

        // Get first campaign and project if exist
        var campaign = await _campaignRepository.FirstOrDefaultAsync();
        var project = await _projectRepository.FirstOrDefaultAsync();

        var baseDate = DateTime.UtcNow.AddMonths(-3);
        int refSeq = 1001;

        // Create 10 sample donations
        var donations = new[]
        {
            // 1. Bollettino Postale Verificato
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors[0].Id,
                DonationChannel.PostalOrder,
                150.00m,
                baseDate.AddDays(5),
                baseDate.AddDays(7),
                campaign?.Id,
                bankAccount.Id,
                "Donazione regolare da bollettino postale"
            ),

            // 2. Bonifico Bancario Verificato
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors[0].Id,
                DonationChannel.BankTransfer,
                500.00m,
                baseDate.AddDays(10),
                baseDate.AddDays(11),
                campaign?.Id,
                bankAccount.Id,
                "Bonifico per progetto specifico"
            ),

            // 3. PayPal Pending
            Donation.CreatePending(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 1 ? donors[1].Id : donors[0].Id,
                DonationChannel.PayPal,
                75.50m,
                DateTime.UtcNow.AddDays(-2),
                "PAYPAL-" + DateTime.UtcNow.Ticks,
                null,
                "Donazione da PayPal in attesa di verifica"
            ),

            // 4. Bollettino Telematico Pending
            Donation.CreatePending(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 2 ? donors[2].Id : donors[0].Id,
                DonationChannel.PostalOrderTelematic,
                100.00m,
                DateTime.UtcNow.AddDays(-1),
                "BT-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                null,
                "Bollettino telematico da verificare"
            ),

            // 5. Contanti Verificato
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors[0].Id,
                DonationChannel.Cash,
                50.00m,
                baseDate.AddDays(15),
                baseDate.AddDays(15),
                campaign?.Id,
                null,
                "Donazione in contanti presso sede"
            ),

            // 6. Assegno Verificato
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 1 ? donors[1].Id : donors[0].Id,
                DonationChannel.Check,
                250.00m,
                baseDate.AddDays(20),
                baseDate.AddDays(22),
                null,
                bankAccount.Id,
                "Assegno bancario"
            ),

            // 7. Grande donazione Bonifico
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 2 ? donors[2].Id : donors[0].Id,
                DonationChannel.BankTransfer,
                1500.00m,
                baseDate.AddDays(25),
                baseDate.AddDays(26),
                campaign?.Id,
                bankAccount.Id,
                "Grande donazione annuale"
            ),

            // 8. Bollettino Postale recente
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors[0].Id,
                DonationChannel.PostalOrder,
                80.00m,
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(-3),
                campaign?.Id,
                bankAccount.Id,
                "Bollettino postale"
            ),

            // 9. RID/SDD Verificato
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 1 ? donors[1].Id : donors[0].Id,
                DonationChannel.DirectDebit,
                120.00m,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow.AddDays(-9),
                campaign?.Id,
                bankAccount.Id,
                "Addebito diretto mensile"
            ),

            // 10. PayPal Verificato recente
            Donation.CreateVerified(
                _guidGenerator.Create(),
                null,
                $"DON-2025-{refSeq++:D5}",
                donors.Count > 2 ? donors[2].Id : donors[0].Id,
                DonationChannel.PayPal,
                65.00m,
                DateTime.UtcNow.AddDays(-3),
                DateTime.UtcNow.AddDays(-2),
                null,
                null,
                "Donazione online PayPal"
            ),
        };

        // Allocate projects to some donations
        if (project != null)
        {
            donations[1].AllocateToProject(project.Id, 500.00m);
            donations[6].AllocateToProject(project.Id, 1500.00m);
        }

        // Insert all donations
        foreach (var donation in donations)
        {
            await _donationRepository.InsertAsync(donation, autoSave: true);
        }
    }
}

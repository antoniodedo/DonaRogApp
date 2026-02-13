using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.Enums.Donors;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Domain.Data
{
    /// <summary>
    /// Data seeder per popolare i titoli di cortesia
    /// </summary>
    public class TitleDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Title, Guid> _titleRepository;

        public TitleDataSeedContributor(IRepository<Title, Guid> titleRepository)
        {
            _titleRepository = titleRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Verifica se ci sono già titoli
            if (await _titleRepository.GetCountAsync() > 0)
            {
                return;
            }

            // Crea titoli di default
            var sigMale = Title.Create("SIG", "Signore", "Sig.", Gender.Male, 1);
            sigMale.SetAsDefault(); // Predefinito per maschi
            
            var sigFemale = Title.Create("SIGRA", "Signora", "Sig.ra", Gender.Female, 2);
            sigFemale.SetAsDefault(); // Predefinito per femmine

            var titles = new[]
            {
                sigMale,
                sigFemale,
                Title.Create("DOTT", "Dottore", "Dott.", Gender.Male, 3),
                Title.Create("DOTTSSA", "Dottoressa", "Dott.ssa", Gender.Female, 4),
                Title.Create("ING", "Ingegnere", "Ing.", null, 5),
                Title.Create("ARCH", "Architetto", "Arch.", null, 6),
                Title.Create("AVV", "Avvocato", "Avv.", null, 7),
                Title.Create("PROF", "Professore", "Prof.", Gender.Male, 8),
                Title.Create("PROFSSA", "Professoressa", "Prof.ssa", Gender.Female, 9),
                Title.Create("RAG", "Ragioniere", "Rag.", null, 10),
                Title.Create("GEOM", "Geometra", "Geom.", null, 11),
                Title.Create("DON", "Don", "Don", Gender.Male, 12),
                Title.Create("PADRE", "Padre", "P.", Gender.Male, 13),
                Title.Create("SUOR", "Suora", "Sr.", Gender.Female, 14),
                Title.Create("MONS", "Monsignore", "Mons.", Gender.Male, 15)
            };

            foreach (var title in titles)
            {
                await _titleRepository.InsertAsync(title);
            }
        }
    }
}

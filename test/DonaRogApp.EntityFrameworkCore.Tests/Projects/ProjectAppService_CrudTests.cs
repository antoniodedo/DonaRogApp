using System;
using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Projects.Dto;
using DonaRogApp.Application.Projects;
using DonaRogApp.EntityFrameworkCore;
using DonaRogApp.Enums.Projects;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace DonaRogApp.Projects
{
    public class ProjectAppService_CrudTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly ProjectAppService _appService;

        public ProjectAppService_CrudTests()
        {
            _appService = GetRequiredService<ProjectAppService>();
        }

        [Fact]
        public async Task Should_Create_Project()
        {
            var input = new CreateProjectDto
            {
                Code = "TEST001",
                Name = "Test Project",
                Description = "Test Description",
                Category = ProjectCategory.Health,
                StartDate = DateTime.Now,
                TargetAmount = 10000m,
                Currency = "EUR"
            };

            var result = await _appService.CreateAsync(input);

            result.ShouldNotBeNull();
            result.Code.ShouldBe("TEST001");
            result.Name.ShouldBe("Test Project");
            result.Category.ShouldBe(ProjectCategory.Health);
        }

        [Fact]
        public async Task Should_Update_Project()
        {
            var createInput = new CreateProjectDto
            {
                Code = "TEST002",
                Name = "Original Name",
                Category = ProjectCategory.Education,
                StartDate = DateTime.Now,
                Currency = "EUR"
            };

            var created = await _appService.CreateAsync(createInput);

            var updateInput = new UpdateProjectDto
            {
                Code = "TEST002",
                Name = "Updated Name",
                Category = ProjectCategory.Education,
                Status = ProjectStatus.Active,
                StartDate = created.StartDate,
                Currency = "EUR"
            };

            var updated = await _appService.UpdateAsync(created.Id, updateInput);

            updated.Name.ShouldBe("Updated Name");
        }

        [Fact]
        public async Task Should_Delete_Project()
        {
            var input = new CreateProjectDto
            {
                Code = "TEST003",
                Name = "To Delete",
                Category = ProjectCategory.Environment,
                StartDate = DateTime.Now,
                Currency = "EUR"
            };

            var created = await _appService.CreateAsync(input);

            await _appService.DeleteAsync(created.Id);

            await Should.ThrowAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
            {
                await _appService.GetAsync(created.Id);
            });
        }

        [Fact]
        public async Task Should_Get_Project_By_Id()
        {
            var input = new CreateProjectDto
            {
                Code = "TEST004",
                Name = "Get Test",
                Category = ProjectCategory.SocialWelfare,
                StartDate = DateTime.Now,
                Currency = "EUR"
            };

            var created = await _appService.CreateAsync(input);

            var retrieved = await _appService.GetAsync(created.Id);

            retrieved.ShouldNotBeNull();
            retrieved.Id.ShouldBe(created.Id);
            retrieved.Code.ShouldBe("TEST004");
        }
    }
}

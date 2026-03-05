using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Projects.Dto;
using DonaRogApp.Application.Projects;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DonaRogApp.Projects
{
    public class ProjectAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly ProjectAppService _appService;

        public ProjectAppService_BasicTests()
        {
            _appService = GetRequiredService<ProjectAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Projects()
        {
            var result = await _appService.GetListAsync(new GetProjectsInput
            {
                MaxResultCount = 10
            });

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetList_Should_Return_Valid_Structure()
        {
            var result = await _appService.GetListAsync(new GetProjectsInput());

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}

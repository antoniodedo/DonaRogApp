using System.Threading.Tasks;
using DonaRogApp.Application.LetterTemplates;
using DonaRogApp.EntityFrameworkCore;
using DonaRogApp.LetterTemplates.Dto;
using Shouldly;
using Xunit;

namespace DonaRogApp.LetterTemplates
{
    public class LetterTemplateAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly LetterTemplateAppService _appService;

        public LetterTemplateAppService_BasicTests()
        {
            _appService = GetRequiredService<LetterTemplateAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Templates()
        {
            var result = await _appService.GetListAsync(new GetLetterTemplatesInput
            {
                MaxResultCount = 10
            });

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetList_Should_Return_PagedResult()
        {
            var result = await _appService.GetListAsync(new GetLetterTemplatesInput());

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}

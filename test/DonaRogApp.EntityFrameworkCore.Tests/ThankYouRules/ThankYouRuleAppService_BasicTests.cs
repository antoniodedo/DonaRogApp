using System.Threading.Tasks;
using DonaRogApp.Application.Communications.ThankYouRules;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DonaRogApp.ThankYouRules
{
    public class ThankYouRuleAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly ThankYouRuleAppService _appService;

        public ThankYouRuleAppService_BasicTests()
        {
            _appService = GetRequiredService<ThankYouRuleAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Rules()
        {
            var result = await _appService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 10
            });

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetList_Should_Return_PagedResult()
        {
            var result = await _appService.GetListAsync(new PagedAndSortedResultRequestDto());

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}

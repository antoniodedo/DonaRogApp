using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Segmentation.Dto;
using DonaRogApp.Application.Segmentation;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DonaRogApp.Segmentation
{
    public class SegmentationRuleAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly SegmentationRuleAppService _appService;

        public SegmentationRuleAppService_BasicTests()
        {
            _appService = GetRequiredService<SegmentationRuleAppService>();
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
            var result = await _appService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10
            });

            result.ShouldBeOfType<PagedResultDto<SegmentationRuleDto>>();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}

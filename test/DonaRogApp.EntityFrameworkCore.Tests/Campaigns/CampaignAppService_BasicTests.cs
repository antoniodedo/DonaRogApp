using System.Threading.Tasks;
using DonaRogApp.Application.Campaigns;
using DonaRogApp.Application.Contracts.Campaigns.Dto;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DonaRogApp.Campaigns
{
    public class CampaignAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly CampaignAppService _appService;

        public CampaignAppService_BasicTests()
        {
            _appService = GetRequiredService<CampaignAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Campaigns()
        {
            var result = await _appService.GetListAsync(new GetCampaignsInput
            {
                MaxResultCount = 10
            });

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }
    }
}

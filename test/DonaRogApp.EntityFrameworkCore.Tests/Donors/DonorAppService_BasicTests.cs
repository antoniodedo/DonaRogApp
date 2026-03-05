using System.Threading.Tasks;
using DonaRogApp.Donors;
using DonaRogApp.Donors.Dto;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DonaRogApp.Donors
{
    public class DonorAppService_BasicTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly DonorAppService _appService;

        public DonorAppService_BasicTests()
        {
            _appService = GetRequiredService<DonorAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_Donors()
        {
            var result = await _appService.GetListAsync(new GetDonorsInput
            {
                MaxResultCount = 10
            });

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetList_Should_Return_Valid_Count()
        {
            var result = await _appService.GetListAsync(new GetDonorsInput());

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using DonaRogApp.Application.Contracts.Segmentation.Dto;
using DonaRogApp.Application.Segmentation;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.EntityFrameworkCore;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DonaRogApp.Segmentation
{
    public class SegmentationRuleAppService_CrudTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly SegmentationRuleAppService _appService;
        private readonly IRepository<Segment, Guid> _segmentRepository;

        public SegmentationRuleAppService_CrudTests()
        {
            _appService = GetRequiredService<SegmentationRuleAppService>();
            _segmentRepository = GetRequiredService<IRepository<Segment, Guid>>();
        }

        [Fact]
        public async Task Should_Create_Segmentation_Rule()
        {
            var segment = await CreateTestSegmentAsync();

            var input = new CreateUpdateSegmentationRuleDto
            {
                Name = "Test Rule",
                Description = "Test Description",
                IsActive = true,
                Priority = 10,
                SegmentId = segment.Id,
                MinRecencyScore = 3,
                MaxRecencyScore = 5
            };

            var result = await _appService.CreateAsync(input);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test Rule");
            result.IsActive.ShouldBeTrue();
            result.Priority.ShouldBe(10);
        }

        [Fact]
        public async Task Should_Update_Segmentation_Rule()
        {
            var segment = await CreateTestSegmentAsync();

            var createInput = new CreateUpdateSegmentationRuleDto
            {
                Name = "Original Rule",
                IsActive = true,
                Priority = 5,
                SegmentId = segment.Id
            };

            var created = await _appService.CreateAsync(createInput);

            var updateInput = new CreateUpdateSegmentationRuleDto
            {
                Name = "Updated Rule",
                IsActive = false,
                Priority = 20,
                SegmentId = segment.Id
            };

            var updated = await _appService.UpdateAsync(created.Id, updateInput);

            updated.Name.ShouldBe("Updated Rule");
            updated.IsActive.ShouldBeFalse();
            updated.Priority.ShouldBe(20);
        }

        [Fact]
        public async Task Should_Delete_Segmentation_Rule()
        {
            var segment = await CreateTestSegmentAsync();

            var input = new CreateUpdateSegmentationRuleDto
            {
                Name = "To Delete",
                Priority = 1,
                SegmentId = segment.Id
            };

            var created = await _appService.CreateAsync(input);

            await _appService.DeleteAsync(created.Id);

            await Should.ThrowAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
            {
                await _appService.GetAsync(created.Id);
            });
        }

        [Fact]
        public async Task Should_Get_Rule_By_Id()
        {
            var segment = await CreateTestSegmentAsync();

            var input = new CreateUpdateSegmentationRuleDto
            {
                Name = "Get Test",
                Priority = 1,
                SegmentId = segment.Id
            };

            var created = await _appService.CreateAsync(input);

            var retrieved = await _appService.GetAsync(created.Id);

            retrieved.ShouldNotBeNull();
            retrieved.Id.ShouldBe(created.Id);
            retrieved.Name.ShouldBe("Get Test");
        }

        private async Task<Segment> CreateTestSegmentAsync()
        {
            var segment = Segment.Create(
                $"TEST_{Guid.NewGuid().ToString().Substring(0, 8)}",
                $"Test Segment {Guid.NewGuid()}"
            );

            return await _segmentRepository.InsertAsync(segment, autoSave: true);
        }
    }
}

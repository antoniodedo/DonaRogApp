using System;
using System.Threading.Tasks;
using DonaRogApp.Application.LetterTemplates;
using DonaRogApp.EntityFrameworkCore;
using DonaRogApp.Enums.Communications;
using DonaRogApp.LetterTemplates.Dto;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace DonaRogApp.LetterTemplates
{
    public class LetterTemplateAppService_CrudTests : DonaRogAppEntityFrameworkCoreTestBase
    {
        private readonly LetterTemplateAppService _appService;

        public LetterTemplateAppService_CrudTests()
        {
            _appService = GetRequiredService<LetterTemplateAppService>();
        }

        [Fact]
        public async Task Should_Create_Template()
        {
            var input = new CreateUpdateLetterTemplateDto
            {
                Name = "Test Template",
                Description = "Test Description",
                Category = TemplateCategory.ThankYou,
                Language = "it",
                IsActive = true,
                Content = "<html><body>Thank you!</body></html>"
            };

            var result = await _appService.CreateAsync(input);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test Template");
            result.Category.ShouldBe(TemplateCategory.ThankYou);
        }

        [Fact]
        public async Task Should_Update_Template()
        {
            var createInput = new CreateUpdateLetterTemplateDto
            {
                Name = "Original Template",
                Category = TemplateCategory.ThankYou,
                Language = "it",
                IsActive = true,
                Content = "<html><body>Original</body></html>"
            };

            var created = await _appService.CreateAsync(createInput);

            var updateInput = new CreateUpdateLetterTemplateDto
            {
                Name = "Updated Template",
                Category = TemplateCategory.ThankYou,
                Language = "it",
                IsActive = true,
                Content = "<html><body>Updated</body></html>"
            };

            var updated = await _appService.UpdateAsync(created.Id, updateInput);

            updated.Name.ShouldBe("Updated Template");
        }

        [Fact]
        public async Task Should_Delete_Template()
        {
            var input = new CreateUpdateLetterTemplateDto
            {
                Name = "To Delete",
                Category = TemplateCategory.ThankYou,
                Language = "it",
                IsActive = true,
                Content = "<html><body>Delete me</body></html>"
            };

            var created = await _appService.CreateAsync(input);

            await _appService.DeleteAsync(created.Id);

            await Should.ThrowAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
            {
                await _appService.GetAsync(created.Id);
            });
        }

        [Fact]
        public async Task Should_Get_Template_By_Id()
        {
            var input = new CreateUpdateLetterTemplateDto
            {
                Name = "Get Test",
                Category = TemplateCategory.ThankYou,
                Language = "it",
                IsActive = true,
                Content = "<html><body>Get me</body></html>"
            };

            var created = await _appService.CreateAsync(input);

            var retrieved = await _appService.GetAsync(created.Id);

            retrieved.ShouldNotBeNull();
            retrieved.Id.ShouldBe(created.Id);
            retrieved.Name.ShouldBe("Get Test");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DonaRogApp.Domain.Shared.Entities;
using DonaRogApp.Tags.Dto;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DonaRogApp.Tags
{
    public class TagAppService : ApplicationService, ITagAppService
    {
        private readonly IRepository<Tag, Guid> _tagRepository;

        public TagAppService(IRepository<Tag, Guid> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<TagDto> GetAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            return MapToDto(tag);
        }

        public async Task<PagedResultDto<TagDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _tagRepository.GetQueryableAsync();

            var totalCount = queryable.Count();

            var sorting = string.IsNullOrEmpty(input.Sorting) ? "Name" : input.Sorting;
            
            // Apply sorting manually
            queryable = sorting.ToLower() switch
            {
                "name" => queryable.OrderBy(x => x.Name),
                "name desc" => queryable.OrderByDescending(x => x.Name),
                "usagecount" => queryable.OrderBy(x => x.UsageCount),
                "usagecount desc" => queryable.OrderByDescending(x => x.UsageCount),
                "category" => queryable.OrderBy(x => x.Category),
                "category desc" => queryable.OrderByDescending(x => x.Category),
                "isactive" => queryable.OrderBy(x => x.IsActive),
                "isactive desc" => queryable.OrderByDescending(x => x.IsActive),
                _ => queryable.OrderBy(x => x.Name)
            };

            var items = queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<TagDto>(
                totalCount,
                items.Select(MapToDto).ToList()
            );
        }

        public async Task<List<TagDto>> GetActiveListAsync()
        {
            var queryable = await _tagRepository.GetQueryableAsync();
            var tags = queryable
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToList();

            return tags.Select(MapToDto).ToList();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var queryable = await _tagRepository.GetQueryableAsync();
            return queryable
                .Where(t => t.Category != null)
                .Select(t => t.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public async Task<TagDto> CreateAsync(CreateUpdateTagDto input)
        {
            // Check if tag with same name already exists
            var queryable = await _tagRepository.GetQueryableAsync();
            var exists = queryable.Any(t => t.Name.ToLower() == input.Name.ToLower());
            if (exists)
            {
                throw new UserFriendlyException("Esiste già un tag con questo nome");
            }

            var tag = Tag.Create(
                input.Name,
                input.Description,
                input.ColorCode,
                input.Category
            );

            await _tagRepository.InsertAsync(tag);

            return MapToDto(tag);
        }

        public async Task<TagDto> UpdateAsync(Guid id, CreateUpdateTagDto input)
        {
            var tag = await _tagRepository.GetAsync(id);

            // Check if another tag with same name exists
            var queryable = await _tagRepository.GetQueryableAsync();
            var exists = queryable.Any(t => t.Id != id && t.Name.ToLower() == input.Name.ToLower());
            if (exists)
            {
                throw new UserFriendlyException("Esiste già un tag con questo nome");
            }

            tag.Update(input.Name, input.Description, input.ColorCode, input.Category);

            await _tagRepository.UpdateAsync(tag);

            return MapToDto(tag);
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            if (tag.IsSystem)
            {
                throw new UserFriendlyException("Impossibile eliminare un tag di sistema");
            }

            if (tag.UsageCount > 0)
            {
                throw new UserFriendlyException($"Impossibile eliminare il tag '{tag.Name}' perché è assegnato a {tag.UsageCount} donatori");
            }

            await _tagRepository.DeleteAsync(tag);
        }

        public async Task ActivateAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            tag.Activate();
            await _tagRepository.UpdateAsync(tag);
        }

        public async Task DeactivateAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            tag.Deactivate();
            await _tagRepository.UpdateAsync(tag);
        }

        private TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Code = tag.Code,
                Name = tag.Name,
                Description = tag.Description,
                ColorCode = tag.ColorCode,
                Category = tag.Category,
                UsageCount = tag.UsageCount,
                IsActive = tag.IsActive,
                IsSystem = tag.IsSystem
            };
        }
    }
}

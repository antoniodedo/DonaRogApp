using DonaRogApp.Application.Contracts.Segmentation;
using DonaRogApp.Application.Contracts.Segmentation.Dto;
using DonaRogApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.HttpApi.Controllers
{
    /// <summary>
    /// Controller for Segmentation Rules
    /// </summary>
    [Route("api/segmentation-rules")]
    [ApiController]
    public class SegmentationRuleController : DonaRogAppController
    {
        private readonly ISegmentationRuleAppService _service;

        public SegmentationRuleController(ISegmentationRuleAppService service)
        {
            _service = service;
        }

        // ======================================================================
        // CRUD
        // ======================================================================

        [HttpGet]
        public virtual Task<PagedResultDto<SegmentationRuleDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
        {
            return _service.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<SegmentationRuleDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<SegmentationRuleDto> CreateAsync([FromBody] CreateUpdateSegmentationRuleDto input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<SegmentationRuleDto> UpdateAsync(Guid id, [FromBody] CreateUpdateSegmentationRuleDto input)
        {
            return _service.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _service.DeleteAsync(id);
        }

        // ======================================================================
        // CUSTOM ACTIONS
        // ======================================================================

        [HttpPost]
        [Route("{id}/toggle-active")]
        public virtual Task<SegmentationRuleDto> ToggleActiveAsync(Guid id)
        {
            return _service.ToggleActiveAsync(id);
        }

        [HttpPost]
        [Route("reorder")]
        public virtual Task ReorderRulesAsync([FromBody] List<RuleOrderDto> order)
        {
            return _service.ReorderRulesAsync(order);
        }

        [HttpGet]
        [Route("{id}/preview")]
        public virtual Task<SegmentEvaluationPreviewDto> PreviewRuleAsync(Guid id, [FromQuery] int maxResults = 100)
        {
            return _service.PreviewRuleAsync(id, maxResults);
        }

        [HttpPost]
        [Route("{id}/apply")]
        public virtual Task<int> ApplyRuleManuallyAsync(Guid id)
        {
            return _service.ApplyRuleManuallyAsync(id);
        }

        [HttpPost]
        [Route("run-batch")]
        public virtual Task<SegmentationBatchResultDto> RunSegmentationBatchAsync()
        {
            return _service.RunSegmentationBatchAsync();
        }

        [HttpGet]
        [Route("last-batch-result")]
        public virtual Task<SegmentationBatchResultDto?> GetLastBatchResultAsync()
        {
            return _service.GetLastBatchResultAsync();
        }

        [HttpGet]
        [Route("available-segments")]
        public virtual Task<List<SegmentDto>> GetAvailableSegmentsAsync()
        {
            return _service.GetAvailableSegmentsAsync();
        }
    }
}

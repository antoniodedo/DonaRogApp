using DonaRogApp.Application.Contracts.Communications.ThankYouRules;
using DonaRogApp.Application.Contracts.Communications.ThankYouRules.Dto;
using DonaRogApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.HttpApi.Controllers
{
    /// <summary>
    /// Controller for Thank You Rules
    /// </summary>
    [Route("api/thank-you-rules")]
    [ApiController]
    public class ThankYouRuleController : DonaRogAppController
    {
        private readonly IThankYouRuleAppService _service;

        public ThankYouRuleController(IThankYouRuleAppService service)
        {
            _service = service;
        }

        // ======================================================================
        // CRUD
        // ======================================================================

        [HttpGet]
        public virtual Task<PagedResultDto<ThankYouRuleDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return _service.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ThankYouRuleDto> GetAsync(Guid id)
        {
            return _service.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ThankYouRuleDto> CreateAsync(CreateUpdateThankYouRuleDto input)
        {
            return _service.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ThankYouRuleDto> UpdateAsync(Guid id, CreateUpdateThankYouRuleDto input)
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
        // RULE EVALUATION
        // ======================================================================

        [HttpPost]
        [Route("evaluate")]
        public virtual Task<ThankYouRuleEvaluationResultDto> EvaluateRulesAsync(EvaluateThankYouRulesDto input)
        {
            return _service.EvaluateRulesAsync(input);
        }

        // ======================================================================
        // CUSTOM ACTIONS
        // ======================================================================

        [HttpPost]
        [Route("{id}/toggle-active")]
        public virtual Task<ThankYouRuleDto> ToggleActiveAsync(Guid id)
        {
            return _service.ToggleActiveAsync(id);
        }

        [HttpPost]
        [Route("reorder")]
        public virtual Task ReorderRulesAsync(List<RuleOrderDto> order)
        {
            return _service.ReorderRulesAsync(order);
        }
    }
}

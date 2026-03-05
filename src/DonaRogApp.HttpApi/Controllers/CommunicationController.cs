using DonaRogApp.Application.Contracts.Communications;
using DonaRogApp.Application.Contracts.Communications.Dto;
using DonaRogApp.Controllers;
using DonaRogApp.Enums.Communications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace DonaRogApp.HttpApi.Controllers
{
    /// <summary>
    /// Controller for Communication management
    /// </summary>
    [Route("api/communications")]
    [ApiController]
    public class CommunicationController : DonaRogAppController
    {
        private readonly ICommunicationAppService _service;

        public CommunicationController(ICommunicationAppService service)
        {
            _service = service;
        }

        // ======================================================================
        // DUPLICATE CHECKS
        // ======================================================================

        [HttpPost]
        [Route("check-duplicates")]
        public virtual Task<DuplicateCheckResultDto> CheckDuplicateLettersAsync([FromBody] CheckDuplicateLettersDto input)
        {
            return _service.CheckDuplicateLettersAsync(input);
        }

        [HttpGet]
        [Route("alert-level/{donorId}")]
        public virtual Task<AlertLevel> GetDuplicateAlertLevelAsync(Guid donorId, int errorThresholdDays = 7, int warningThresholdDays = 15)
        {
            return _service.GetDuplicateAlertLevelAsync(donorId, errorThresholdDays, warningThresholdDays);
        }

        // ======================================================================
        // HISTORY
        // ======================================================================

        [HttpGet]
        [Route("history")]
        public virtual Task<PagedResultDto<CommunicationHistoryDto>> GetHistoryAsync([FromQuery] GetCommunicationHistoryInput input)
        {
            return _service.GetHistoryAsync(input);
        }

        [HttpGet]
        [Route("donor/{donorId}/recent")]
        public virtual Task<List<RecentCommunicationDto>> GetDonorRecentCommunicationsAsync(Guid donorId, int lastDays = 30)
        {
            return _service.GetDonorRecentCommunicationsAsync(donorId, lastDays);
        }
    }
}

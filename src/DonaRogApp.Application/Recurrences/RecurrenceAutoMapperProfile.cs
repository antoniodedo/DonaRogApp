using AutoMapper;
using DonaRogApp.Application.Contracts.Recurrences.Dto;
using DonaRogApp.Domain.Recurrences.Entities;

namespace DonaRogApp.Application.Recurrences
{
    /// <summary>
    /// AutoMapper Profile for Recurrence Aggregate
    /// Configuration for Entity <-> DTO mappings
    /// </summary>
    public class RecurrenceAutoMapperProfile : Profile
    {
        public RecurrenceAutoMapperProfile()
        {
            // ======================================================================
            // RECURRENCE MAPPINGS
            // ======================================================================

            // Entity -> RecurrenceDto (full)
            CreateMap<Recurrence, RecurrenceDto>();

            // Entity -> RecurrenceListDto (simplified)
            CreateMap<Recurrence, RecurrenceListDto>();

            // CreateRecurrenceDto -> Entity (handled in AppService with factory method)

            // UpdateRecurrenceDto -> Entity (handled in AppService with update methods)
        }
    }
}

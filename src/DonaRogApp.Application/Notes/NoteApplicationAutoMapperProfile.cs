using AutoMapper;
using DonaRogApp.Notes.Dto;
using DonaRogApp.Notes.Entities;

namespace DonaRogApp.Notes
{
    public class NoteApplicationAutoMapperProfile : Profile
    {
        public NoteApplicationAutoMapperProfile()
        {
            CreateMap<Note, NoteDto>();
            CreateMap<CreateUpdateNoteDto, Note>();
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DonaRogApp.Notes.Dto
{
    public class CreateUpdateNoteDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(64)]
        public string? Category { get; set; }

        public DateTime? InteractionDate { get; set; }

        public bool IsImportant { get; set; }

        public bool IsPrivate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Learning.Server.Models
{
    public class ParticipantLesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        [ForeignKey("ParticipantId")]
        public virtual Participant Participant { get; set; }

        [Required]
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }

        [Required]
        public int FormationId { get; set; }
        [ForeignKey("FormationId")]
        public virtual Formation Formation { get; set; }

        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
    }
}
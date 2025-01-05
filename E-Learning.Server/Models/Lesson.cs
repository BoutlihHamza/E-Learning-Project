using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_Learning.Server.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        // Content or video URL for the lesson
        public string? ContentUrl { get; set; }

        // Duration in minutes
        public decimal Duration { get; set; }

        // Order of the lesson within the formation
        public int OrderIndex { get; set; }

        // Indicates if this is a preview lesson available before purchase
        public bool IsPreview { get; set; } = false;

        // Foreign key for Formation
        [Required]
        public int FormationId { get; set; }

        [ForeignKey("FormationId")]
        [JsonIgnore]
        public virtual Formation? Formation { get; set; }
        
        public bool IsCompleted { get; set; } = false;

        // Additional metadata
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
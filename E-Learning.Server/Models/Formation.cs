using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace E_Learning.Server.Models
{
    public class Formation
    {
        public Formation()
        {
            ParticipantFormations = new HashSet<ParticipantFormation>();
            certificates = new HashSet<Certificate>();
        }
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }
        public string? Image {  get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public decimal Duration { get; set; }
        public Level? Level { get; set; }
        public Language? Language { get; set; }
        public DateTime Deadline { get; set; }
        public decimal rating {  get; set; } = 0;
        public decimal price { get; set; }
        public decimal oldPrice { get; set; }
        public int? review { get; set; }
        public string? reviewText { get; set; }
        public decimal? passPercentage { get; set; }
        public bool? featured { get; set; }
        public string? certificate { get; set; }
        public string? videoLink { get; set; }
        public string details { get; set; } = "";

        public int FormateurId { get; set; }

        [ForeignKey("FormateurId")]
        public virtual Formateur? Formateur { get; set; }

        
        public virtual ICollection<Lesson> Lessons { get; set; }

        // Navigation property for many-to-many relationship with Participant
        public virtual ICollection<ParticipantFormation> ParticipantFormations { get; set; } = new List<ParticipantFormation>();
        [JsonIgnore]
        public virtual ICollection<Certificate>? certificates { get; set; }
    }
}

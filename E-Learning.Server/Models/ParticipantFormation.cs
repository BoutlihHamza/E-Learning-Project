using E_Learning.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ParticipantFormation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int ParticipantId { get; set; }
    [Required]
    public int FormationId { get; set; }

    [ForeignKey("ParticipantId")]
    public virtual Participant Participant { get; set; }
    [ForeignKey("FormationId")]
    public virtual Formation Formation { get; set; }
    public double Progress { get; set; }
    public DateTime EnrollmentDate { get; set; }
}
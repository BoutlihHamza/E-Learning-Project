using E_Learning.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Certificate
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
    [DeleteBehavior(DeleteBehavior.NoAction)]
    [ForeignKey("FormationId")]
    public virtual Formation Formation { get; set; }
    public DateTime DateIssued { get; set; }
}
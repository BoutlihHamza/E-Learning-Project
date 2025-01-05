using E_Learning.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Panier
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [ForeignKey("ParticipantId")]
    public virtual Participant Participant { get; set; }

    public virtual ICollection<PanierItem> PanierItems { get; set; } = new List<PanierItem>();

    [NotMapped]
    public decimal TotalAmount => PanierItems?.Sum(item =>
        item.UnitPrice - (item.DiscountAmount ?? 0)) ?? 0;

    [NotMapped]
    public int TotalItems => PanierItems?.Count ?? 0;
}

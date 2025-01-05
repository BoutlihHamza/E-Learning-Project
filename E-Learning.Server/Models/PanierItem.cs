using E_Learning.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PanierItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PanierId { get; set; }

    [ForeignKey("PanierId")]
    [JsonIgnore]
    public virtual Panier Panier { get; set; }

    [Required]
    public int FormationId { get; set; }

    [ForeignKey("FormationId")]
    public virtual Formation Formation { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountAmount { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
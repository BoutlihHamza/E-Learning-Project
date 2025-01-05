using E_Learning.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Participant : Utilisateur
{
    public Participant()
    {
        ParticipantFormations = new HashSet<ParticipantFormation>();
        Certificates = new HashSet<Certificate>();
    }

    public virtual ICollection<ParticipantFormation> ParticipantFormations { get; set; }
    public virtual ICollection<Certificate> Certificates { get; set; }
    public int PanierId { get; set; }
    [ForeignKey("PanierId")]
    public virtual Panier panier { get; set; }
    public bool IsPro {  get; set; }
}
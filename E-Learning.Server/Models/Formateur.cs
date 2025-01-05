using System.Text.Json.Serialization;

namespace E_Learning.Server.Models
{
    public class Formateur : Utilisateur
    {
        public string Speciality { get; set; }

        // Navigation property for formations created by the formateur
        public ICollection<Formation>? Formations { get; set; } = new List<Formation>();
    }
}

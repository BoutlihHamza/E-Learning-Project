namespace E_Learning.Server.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Formation>? formations { get; set; }
    }
}
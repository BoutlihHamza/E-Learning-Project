namespace E_Learning.Server.Models.DTOs
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContentUrl { get; set; }
        public decimal Duration { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
    }
}

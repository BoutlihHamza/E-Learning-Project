namespace E_Learning.Server.Models.DTOs
{
    public class CompletedLessonDTO
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public string FormationTitle { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}

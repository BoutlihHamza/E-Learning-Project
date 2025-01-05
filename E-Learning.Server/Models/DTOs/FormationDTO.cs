namespace E_Learning.Server.Models.DTOs
{
    public class FormationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public string? Instructor { get; set; }
        public string Duration { get; set; }        // Format: "8h 15m 30s"
        public string DurationInHour { get; set; }  // Format: "8 Hours"
        public string DurationInHourMinute { get; set; } // Format: "8hr 15min"
        public string? Level { get; set; }
        public string Language { get; set; }
        public string Deadline { get; set; }       // Format: "28 Dec, 2023"
        public string Rating { get; set; }
        public int Student { get; set; }
        public int Lesson { get; set; }
        public int Quizzes { get; set; }
        public string Price { get; set; }           // Format: "55.00"
        public decimal Review { get; set; }
        public string? ReviewText { get; set; }
        public decimal PassPercentage { get; set; }
        public bool Featured { get; set; }
        public string? Certificate { get; set; }
        public string FilterParam { get; set; }
        public List<string> Categories { get; set; }
        public List<string> VideoLink { get; set; }
        public string Excerpt { get; set; }
        public string Details { get; set; }
    }
}
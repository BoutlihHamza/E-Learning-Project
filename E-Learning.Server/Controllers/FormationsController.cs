using E_Learning.Server.Models;
using E_Learning.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Learning.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormationsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public FormationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Formations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormationDTO>>> GetFormations()
        {
            var formations = await _context.Formations
                .Include(f => f.Formateur)
                .Include(f => f.ParticipantFormations)
                .Include(f => f.Category)
                .Include(f => f.Lessons)
                .Select(f => new FormationDTO
                {
                    Id = f.Id,
                    Title = f.Title,
                    Image = f.Image,
                    Instructor = f.Formateur != null ? $"{f.Formateur.FirstName} {f.Formateur.LastName}" : null,
                    Duration = FormatDuration(f.Duration),
                    DurationInHour = FormatDurationInHours(f.Duration),
                    DurationInHourMinute = FormatDurationInHourMinute(f.Duration),
                    Level = f.Level.ToString(),
                    Language = f.Language.ToString(),
                    Deadline = f.Deadline.ToString("dd MMM, yyyy"),
                    Rating = f.rating.ToString("0.0") ?? "0.0",
                    Student = f.ParticipantFormations.Count,
                    Lesson = f.Lessons.Count,
                    Quizzes = 0, // Add Quizzes property to Formation if needed
                    Price = f.price.ToString("0.00"),
                    Review = f.review ?? 0,
                    ReviewText = f.reviewText ?? "",
                    PassPercentage = f.passPercentage ?? 0,
                    Featured = f.featured ?? false,
                    Certificate = f.certificate ?? "not available",
                    FilterParam = "Trending", // Implement your filtering logic
                    Categories = new List<string> { f.Category.Name },
                    VideoLink = f.videoLink != null ? new List<string> { f.videoLink } : new List<string>(),
                    Excerpt = GetExcerpt(f.details),
                    Details = f.details
                })
                .ToListAsync();

            return formations;
        }

        // GET: api/Formations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FormationDTO>> GetFormation(int id)
        {
            var formation = await _context.Formations
                .Include(f => f.Formateur)
                .Include(f => f.ParticipantFormations)
                .Where(f => f.Id == id)
                .Select(f => new FormationDTO
                {
                    Id = f.Id,
                    Title = f.Title,
                    Image = f.Image,
                    Instructor = f.Formateur != null ? $"{f.Formateur.FirstName} {f.Formateur.LastName}" : null,
                    Duration = FormatDuration(f.Duration),
                    DurationInHour = FormatDurationInHours(f.Duration),
                    DurationInHourMinute = FormatDurationInHourMinute(f.Duration),
                    Level = f.Level.ToString(),
                    Language = f.Language.ToString(),
                    Deadline = f.Deadline.ToString("dd MMM, yyyy"),
                    Rating = f.rating.ToString("0.0") ?? "0.0",
                    Student = f.ParticipantFormations.Count,
                    Lesson = f.Lessons.Count,
                    Quizzes = 0, // Add Quizzes property to Formation if needed
                    Price = f.price.ToString("0.00"),
                    Review = f.review ?? 0,
                    ReviewText = f.reviewText ?? "",
                    PassPercentage = f.passPercentage ?? 0,
                    Featured = f.featured ?? false,
                    Certificate = f.certificate ?? "not available",
                    FilterParam = "Trending", // Implement your filtering logic
                    Categories = new List<string> { f.Category.Name },
                    VideoLink = f.videoLink != null ? new List<string> { f.videoLink } : new List<string>(),
                    Excerpt = GetExcerpt(f.details),
                    Details = f.details
                })
                .FirstOrDefaultAsync();

            if (formation == null)
            {
                return NotFound();
            }

            return formation;
        }

        // GET: api/Formations/Category/{category}
        [HttpGet("Category/{category}")]
        public async Task<ActionResult<IEnumerable<FormationDTO>>> GetFormationsByCategory(string category)
        {
            var formations = await _context.Formations
                .Where(f => f.Category.Name == category)
                .Include(f => f.Formateur)
                .Include(f => f.ParticipantFormations)
                .Select(f => new FormationDTO
                {
                    Id = f.Id,
                    Title = f.Title,
                    Image = f.Image,
                    Instructor = f.Formateur != null ? $"{f.Formateur.FirstName} {f.Formateur.LastName}" : null,
                    Duration = FormatDuration(f.Duration),
                    DurationInHour = FormatDurationInHours(f.Duration),
                    DurationInHourMinute = FormatDurationInHourMinute(f.Duration),
                    Level = f.Level.ToString(),
                    Language = f.Language.ToString(),
                    Deadline = f.Deadline.ToString("dd MMM, yyyy"),
                    Rating = f.rating.ToString("0.0") ?? "0.0",
                    Student = f.ParticipantFormations.Count,
                    Lesson = f.Lessons.Count,
                    Quizzes = 0, // Add Quizzes property to Formation if needed
                    Price = f.price.ToString("0.00"),
                    Review = f.review ?? 0,
                    ReviewText = f.reviewText ?? "",
                    PassPercentage = f.passPercentage ?? 0,
                    Featured = f.featured ?? false,
                    Certificate = f.certificate ?? "not available",
                    FilterParam = "Trending", // Implement your filtering logic
                    Categories = new List<string> { f.Category.Name },
                    VideoLink = f.videoLink != null ? new List<string> { f.videoLink } : new List<string>(),
                    Excerpt = GetExcerpt(f.details),
                    Details = f.details
                })
                .ToListAsync();

            return formations;
        }

        // POST: api/Formations
        [HttpPost]
        [Authorize(Roles = "Formateur")] // Using role string
        [Authorize(Policy = "FormateurOnly")]
        public async Task<ActionResult<Formation>> CreateFormation(Formation formation)
        {
            // Get the current user's ID from the token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Set the FormateurId to the current user's ID
            formation.FormateurId = userId;

            //// Check if lessons are provided
            //if (formation.Lessons != null && formation.Lessons.Any())
            //{
            //    foreach (var lesson in formation.Lessons)
            //    {
            //        lesson.Formation = formation; // Ensure the lesson is linked to the formation
            //    }
            //}

            _context.Formations.Add(formation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFormation), new { id = formation.Id }, formation);
        }


        // PUT: api/Formations/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Formateur")]               // Using role string
        [Authorize(Policy = "FormateurOnly")]
        public async Task<IActionResult> UpdateFormation(int id, Formation formation)
        {
            if (id != formation.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Verify that the current user owns this formation
            var existingFormation = await _context.Formations.FindAsync(id);
            if (existingFormation == null)
            {
                return NotFound();
            }

            if (existingFormation.FormateurId != userId)
            {
                return Forbid();
            }

            _context.Entry(existingFormation).State = EntityState.Detached;
            _context.Entry(formation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormationExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Formations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Formateur")]               // Using role string
        [Authorize(Policy = "FormateurOnly")]
        public async Task<IActionResult> DeleteFormation(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Verify that the current user owns this formation
            if (formation.FormateurId != userId)
            {
                return Forbid();
            }

            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Formations/5/Enroll
        [HttpPost("{id}/Enroll")]
        [Authorize(Roles = "Participant")]               // Using role string
        [Authorize(Policy = "ParticipantOnly")]
        public async Task<IActionResult> EnrollInFormation(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Check if already enrolled
            var existingEnrollment = await _context.ParticipantFormations
                .AnyAsync(pf => pf.FormationId == id && pf.ParticipantId == userId);

            if (existingEnrollment)
            {
                return BadRequest("Already enrolled in this formation");
            }

            var participantFormation = new ParticipantFormation
            {
                FormationId = id,
                ParticipantId = userId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.ParticipantFormations.Add(participantFormation);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool FormationExists(int id)
        {
            return _context.Formations.Any(e => e.Id == id);
        }

        // Helper methods for duration formatting
        private static string FormatDuration(decimal totalMinutes)
        {
            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);
            int seconds = (int)((totalMinutes % 1) * 60);

            return $"{hours}h {minutes}m {seconds}s";
        }

        private static string FormatDurationInHours(decimal totalMinutes)
        {
            int hours = (int)Math.Ceiling(totalMinutes / 60);
            return $"{hours} {(hours == 1 ? "Hour" : "Hours")}";
        }

        private static string FormatDurationInHourMinute(decimal totalMinutes)
        {
            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);

            return $"{hours}hr {minutes}min";
        }

        private static string GetExcerpt(string details)
        {
            if (string.IsNullOrEmpty(details))
                return "";

            var plainText = StripHtmlTags(details);
            return plainText.Length > 200
                ? plainText.Substring(0, 197) + "..."
                : plainText;
        }

        private static string StripHtmlTags(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
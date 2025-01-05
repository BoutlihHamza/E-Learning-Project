using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using E_Learning.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using E_Learning.Server.Models.DTOs;

namespace E_Learning.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Formateur")]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new lesson
        [HttpPost("add-lesson")]
        public async Task<IActionResult> AddLesson([FromBody] LessonForCreationDTO lessonDto)
        {
            var formateurId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var formation = await _context.Formations.FindAsync(lessonDto.FormationId);
            if (formation == null)
            {
                return NotFound("Formation not found.");
            }

            if (formation.FormateurId != formateurId)
            {
                return Forbid("You are not authorized to add lessons to this formation.");
            }

            var lesson = new Lesson
            {
                Title = lessonDto.Title,
                Description = lessonDto.Description,
                ContentUrl = lessonDto.ContentUrl,
                Duration = lessonDto.Duration,
                OrderIndex = lessonDto.OrderIndex,
                IsPreview = lessonDto.IsPreview,
                FormationId = lessonDto.FormationId
            };

            formation.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            var lessonDTO = new LessonDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderIndex = lesson.OrderIndex,
                IsPreview = lesson.IsPreview
            };

            return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lessonDTO);
        }

        // Get all lessons for a formation
        [HttpGet("formation/{formationId}")]
        public async Task<ActionResult<IEnumerable<LessonDTO>>> GetLessonsForFormation(int formationId)
        {
            var formation = await _context.Formations
                .Include(f => f.Lessons)
                .ThenInclude(l => l.Formation)
                .FirstOrDefaultAsync(f => f.Id == formationId);

            if (formation == null)
            {
                return NotFound("Formation not found.");
            }

            var lessons = formation.Lessons
                .OrderBy(l => l.OrderIndex)
                .Select(l => new LessonDTO
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    ContentUrl = l.ContentUrl,
                    Duration = l.Duration,
                    OrderIndex = l.OrderIndex,
                    IsPreview = l.IsPreview
                })
                .ToList();

            return Ok(lessons);
        }

        // Get a single lesson by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDTO>> GetLesson(int id)
        {
            var lesson = await _context.lessons.FindAsync(id);

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            var lessonDto = new LessonDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderIndex = lesson.OrderIndex,
                IsPreview = lesson.IsPreview
            };

            return Ok(lessonDto);
        }

        // Update a lesson
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonForCreationDTO lessonDto)
        {
            var formateurId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var lesson = await _context.lessons
                .Include(l => l.Formation)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            // Verify the formateur owns the formation
            if (lesson.Formation.FormateurId != formateurId)
            {
                return Forbid("You are not authorized to update this lesson.");
            }

            // Update lesson properties
            lesson.Title = lessonDto.Title;
            lesson.Description = lessonDto.Description;
            lesson.ContentUrl = lessonDto.ContentUrl;
            lesson.Duration = lessonDto.Duration;
            lesson.OrderIndex = lessonDto.OrderIndex;
            lesson.IsPreview = lessonDto.IsPreview;

            _context.lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete a lesson
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var formateurId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var lesson = await _context.lessons
                .Include(l => l.Formation)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            // Verify the formateur owns the formation
            if (lesson.Formation.FormateurId != formateurId)
            {
                return Forbid("You are not authorized to delete this lesson.");
            }

            _context.lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
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
    [Authorize(Roles = "Participant")]
    public class LessonCompletionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonCompletionController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetAuthenticatedParticipantId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Extract participant ID from token
        }

        [HttpPost("completelesson")]
        public async Task<IActionResult> CompleteLesson([FromBody] CompleteLessonDTO model)
        {
            var participantId = GetAuthenticatedParticipantId();
            var lessonId = model.LessonId;

            var lesson = await _context.lessons
                .Include(l => l.Formation)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return NotFound("Lesson not found");

            var formationId = lesson.FormationId;

            // Check if the participant is enrolled in the formation
            var participantFormation = await _context.ParticipantFormations
                .FirstOrDefaultAsync(pf => pf.ParticipantId == participantId && pf.FormationId == formationId);

            if (participantFormation == null)
            {
                return NotFound("Participant is not enrolled in this formation.");
            }

            // Check if the lesson is already completed by the participant
            var existingCompletion = await _context.ParticipantLessons
                .FirstOrDefaultAsync(pl => pl.ParticipantId == participantId && pl.LessonId == lessonId && pl.FormationId == formationId);

            if (existingCompletion != null)
            {
                return Conflict("This lesson has already been completed by the participant.");
            }

            // Create a new ParticipantLesson record
            var participantLesson = new ParticipantLesson
            {
                ParticipantId = participantId,
                LessonId = lessonId,
                FormationId = formationId,
                CompletedDate = DateTime.UtcNow
            };
            

            _context.ParticipantLessons.Add(participantLesson);
            await _context.SaveChangesAsync();

            // Calculate progress
            var totalLessons = await _context.lessons
                .CountAsync(l => l.FormationId == formationId);

            var completedLessons = await _context.ParticipantLessons
                .CountAsync(pl => pl.ParticipantId == participantId && pl.FormationId == formationId);

            var progress = (double)completedLessons / totalLessons * 100;
            // Update progress in ParticipantFormation
            participantFormation.Progress = progress;
            _context.ParticipantFormations.Update(participantFormation);
            await _context.SaveChangesAsync();

            return Ok("Lesson marked as completed successfully.");
        }

        [HttpGet("completedlessons")]
        public async Task<IActionResult> GetCompletedLessons()
        {
            var participantId = GetAuthenticatedParticipantId();

            // Fetch completed lessons for the participant
            var completedLessons = await _context.ParticipantLessons
                .Where(pl => pl.ParticipantId == participantId)
                .Include(pl => pl.Lesson)
                .Include(pl => pl.Formation)
                .Select(pl => new CompletedLessonDTO
                {
                    LessonId = pl.LessonId,
                    LessonTitle = pl.Lesson.Title,
                    FormationTitle = pl.Formation.Title,
                    CompletedDate = pl.CompletedDate
                })
                .ToListAsync();

            return Ok(completedLessons);
        }
    }
}
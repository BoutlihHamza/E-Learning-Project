using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using E_Learning.Server.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Participant")] // Restrict access to participants only
public class CertificatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CertificatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper method to get the authenticated participant's ID
    private int GetAuthenticatedParticipantId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Extract participant ID from token
    }

    // GET: api/Certificates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
    {
        int participantId = GetAuthenticatedParticipantId();

        return await _context.Certificates
            .Where(c => c.ParticipantId == participantId) // Restrict to authenticated participant
            .Include(c => c.Formation)
            .ToListAsync();
    }

    // GET: api/Certificates/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Certificate>> GetCertificate(int id)
    {
        int participantId = GetAuthenticatedParticipantId();

        var certificate = await _context.Certificates
            .Include(c => c.Formation)
            .FirstOrDefaultAsync(c => c.Id == id && c.ParticipantId == participantId); // Restrict to participant

        if (certificate == null)
        {
            return NotFound(new { Message = "Certificate not found or access denied." });
        }

        return certificate;
    }

    // POST: api/Certificates/Generate
    [HttpPost("Generate")]
    public async Task<IActionResult> GenerateCertificate([FromBody] int formationId)
    {
        int participantId = GetAuthenticatedParticipantId();

        // Check if the formation exists
        var formation = await _context.Formations.FindAsync(formationId);
        if (formation == null)
        {
            return NotFound(new { Message = "Formation not found" });
        }

        // Check if progress is 100%
        var participantFormation = await _context.ParticipantFormations
            .FirstOrDefaultAsync(pf => pf.ParticipantId == participantId && pf.FormationId == formationId);

        if (participantFormation == null || participantFormation.Progress < 100)
        {
            return BadRequest(new { Message = "Certificate cannot be issued. Progress is incomplete." });
        }

        // Check if the certificate already exists
        var existingCertificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.ParticipantId == participantId && c.FormationId == formationId);

        if (existingCertificate != null)
        {
            return BadRequest(new { Message = "Certificate already exists." });
        }

        // Generate certificate
        var certificate = new Certificate
        {
            ParticipantId = participantId,
            FormationId = formationId,
            DateIssued = DateTime.UtcNow
        };

        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Certificate generated successfully", Certificate = certificate });
    }

    // DELETE: api/Certificates/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCertificate(int id)
    {
        int participantId = GetAuthenticatedParticipantId();

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == id && c.ParticipantId == participantId); // Restrict to participant

        if (certificate == null)
        {
            return NotFound(new { Message = "Certificate not found or access denied." });
        }

        _context.Certificates.Remove(certificate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

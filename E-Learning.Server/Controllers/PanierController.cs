using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using E_Learning.Server.Models;
using E_Learning.Server.Models.DTOs;

namespace E_Learning.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Participant")] // Only participants can access this controller
    public class PanierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get the current participant's ID from claims
        private int GetCurrentParticipantId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID claim");
            }
            return userId;
        }

        // GET: api/Panier
        [HttpGet]
        public async Task<ActionResult<Panier>> GetMyPanier()
        {
            int participantId = GetCurrentParticipantId();

            var panier = await _context.Paniers
                .Include(p => p.PanierItems)
                    .ThenInclude(pi => pi.Formation)
                .FirstOrDefaultAsync(p => p.ParticipantId == participantId);

            if (panier == null)
            {
                // Create a new cart if none exists
                panier = new Panier { ParticipantId = participantId };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            return panier;
        }

        [HttpPost("addItem")]
        public async Task<ActionResult<PanierItem>> AddItemToPanier([FromBody] AddPanierItemRequest request)
        {
            int participantId = GetCurrentParticipantId();

            var panier = await _context.Paniers
                .Include(p => p.PanierItems)
                .FirstOrDefaultAsync(p => p.ParticipantId == participantId);

            if (panier == null)
            {
                panier = new Panier { ParticipantId = participantId };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            // Check if course already exists in cart
            var existingItem = panier.PanierItems
                .FirstOrDefault(pi => pi.FormationId == request.FormationId);

            if (existingItem != null)
            {
                return BadRequest("This course is already in your cart");
            }

            var formation = await _context.Formations
                .FirstOrDefaultAsync(f => f.Id == request.FormationId);

            if (formation == null)
            {
                return NotFound("Formation not found");
            }

            var newItem = new PanierItem
            {
                PanierId = panier.Id,
                FormationId = request.FormationId,
                UnitPrice = formation.price,
                DiscountAmount = request.DiscountAmount
            };

            _context.PanierItems.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok(await GetMyPanier());
        }


        // DELETE: api/Panier/removeItem/{itemId}
        [HttpDelete("removeItem/{itemId}")]
        public async Task<ActionResult> RemoveItem(int itemId)
        {
            int participantId = GetCurrentParticipantId();

            var panierItem = await _context.PanierItems
                .Include(pi => pi.Panier)
                .FirstOrDefaultAsync(pi => pi.Id == itemId);

            if (panierItem == null)
            {
                return NotFound();
            }

            // Verify the item belongs to the current user's cart
            if (panierItem.Panier.ParticipantId != participantId)
            {
                return Forbid();
            }

            _context.PanierItems.Remove(panierItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Panier/clear
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearPanier()
        {
            int participantId = GetCurrentParticipantId();

            var panier = await _context.Paniers
                .Include(p => p.PanierItems)
                .FirstOrDefaultAsync(p => p.ParticipantId == participantId);

            if (panier == null)
            {
                return NotFound();
            }

            _context.PanierItems.RemoveRange(panier.PanierItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}
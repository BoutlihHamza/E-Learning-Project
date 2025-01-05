using E_Learning.Server.Helpers;
using E_Learning.Server.Models;
using E_Learning.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<Utilisateur> userManager;
    private readonly SignInManager<Utilisateur> signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> logger;

    public UserController(
        UserManager<Utilisateur> userManager,
        SignInManager<Utilisateur> signInManager,
        ILogger<UserController> logger,
        ApplicationDbContext context)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this._context = context;
    }


    [HttpPost("add-user")]
    public async Task<IActionResult> Register(RegistrationModel model)
    {
        string added = "Utilisateur";
        var user = new Utilisateur()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Role = model.Role,
            UserName = model.Email,
            PasswordHash = model.Password
        };

        if (((int)model.Role) == 3)
        {
            added = "Formateur";
            user = new Formateur()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                UserName = model.Email,
                PasswordHash = model.Password,
                Speciality = model.Speciality,
                Formations = new List<Formation>()
            };
        }
        else if ((int)model.Role == 2)
        {
            added = "Participant";

            user = new Participant()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                UserName = model.Email,
                PasswordHash = model.Password,
                ParticipantFormations = new List<ParticipantFormation>(),
                Certificates = new List<Certificate>()
            };
        }

        var result = await userManager.CreateAsync(user, user.PasswordHash!);
        if (result.Succeeded)
        {
            if (user is Participant participant)
            {
                var panier = new Panier()
                {
                    ParticipantId = participant.Id,
                };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
                participant.panier = panier;
                participant.PanierId = participant.Id;
                await _context.SaveChangesAsync();
            }

            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleHelper.GetRoleString(user.Role)));
            return Ok($"{added} registered successfully");
        }
        return BadRequest("Error Occured");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
        {
            // First try to find the user
            var user = await userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                logger.LogWarning($"Login attempt failed: User not found for email {model.Email}");
                return BadRequest("Invalid login attempt");
            }

            var signInResult = await signInManager.PasswordSignInAsync(
                user,  // Pass the user object directly
                model.Password!,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (signInResult.Succeeded)
            {
                logger.LogInformation($"User {model.Email} logged in successfully");
                return Ok(new
                {
                    Message = "You are logged in successfully",
                    Role = user.Role,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            logger.LogWarning($"Login failed for user {model.Email}: {signInResult}");
            if (signInResult.IsLockedOut)
                return BadRequest("Account is locked out");
            if (signInResult.IsNotAllowed)
                return BadRequest("Login is not allowed");
            if (signInResult.RequiresTwoFactor)
                return BadRequest("Requires two factor authentication");

            return BadRequest("Invalid login attempt");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login attempt");
            return StatusCode(500, "An error occurred during login");
        }
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role
        });
    }
}
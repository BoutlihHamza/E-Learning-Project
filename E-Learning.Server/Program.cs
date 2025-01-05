using E_Learning.Server.Helpers;
using E_Learning.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity API Endpoints with configuration
builder.Services.AddIdentityApiEndpoints<Utilisateur>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//Assia Code
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout duration
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowCredentials() //to allow cookie
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//End ASCD

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FormateurOnly", policy =>
        policy.RequireRole("Formateur"));

    options.AddPolicy("ParticipantOnly", policy =>
        policy.RequireRole("Participant"));
});

var app = builder.Build();
//Assia code
app.UseCors("AllowFrontend");
app.UseSession();
//End ASCD

app.UseDefaultFiles();
app.MapStaticAssets();

//Assia Code
app.MapPost("/register", async (UserManager<Utilisateur> userManager, HttpRequest request) =>
{
    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);


    var roleUser = formData["role"];
    var email = formData["email"];
    var userName = formData["userName"];
    var password = formData["password"];
    var confirmPassword = formData["confirmPassword"];

    if (password != confirmPassword)
    {
        return Results.BadRequest(new { message = "Passwords do not match." });
    }

    Utilisateur user;

    if (roleUser == "Formateur")
    {
        var speciality = formData["speciality"];
        user = new Formateur
        {
            FirstName = userName,
            LastName = userName,
            Email = email,
            UserName = userName,
            Speciality = speciality,
            Role = Role.Formateur
        };
    }
    else if (roleUser == "Participant")
    {
        user = new Participant
        {
            FirstName = userName,
            LastName = userName,
            Email = email,
            UserName = userName,
            Role = Role.Participant,
            IsPro = false
        };
    }
    else if (roleUser == "Admin")
    {
        user = new Utilisateur
        {
            FirstName = userName,
            LastName = userName,
            Email = email,
            UserName = userName,
            Role = 0
        };
    }

    else
    {
        return Results.BadRequest(new { message = "Failed to register the user" });
    }

    var result = await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {

        if (roleUser == "Participant")
        {
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleHelper.GetRoleString(user.Role)));
            return Results.Ok(new { message = "Participant registered successfully" });
        }
        else if (roleUser == "Formateur")
        {
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleHelper.GetRoleString(user.Role)));
            return Results.Ok(new { message = "Formateur registered successfully" });
        }
        else if ( roleUser == "Admin")
        {
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleHelper.GetRoleString(user.Role)));
            return Results.Ok(new { message = "Admin registered successfully" });
        }
        else
        {
                                    
        }
        {
            return Results.BadRequest(new { message = "No role asgned" });
        }

    }
    else
    {
        return Results.BadRequest(new { message = "Failed to creat a user" });
    }

});













// Map Identity API endpoints
app.MapIdentityApi<Utilisateur>();

// Custom logout endpoint
app.MapPost("/logout", async (SignInManager<Utilisateur> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

// Auth test endpoint
app.MapGet("pingauth", (ClaimsPrincipal user) =>
{
    var email = user.FindFirstValue(ClaimTypes.Email);
    return Results.Json(new { Email = email });
}).RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
using AuthOnion.Api.Domain.Entities;
using AuthOnion.Api.Domain.Interfaces;
using AuthOnion.Api.Infrastructure.Data;
using AuthOnion.Api.Infrastructure.Repositories;
using AuthOnion.Api.Infrastructure.Services;
using AuthOnion.Api.Application.Interfaces;
using AuthOnion.Api.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure EF Core InMemory database
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("AuthOnionDb");
});

// Bind JWT settings from appsettings.json
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

// Configure Authentication with JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

// Dependency Injection for repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<IJwtService, JwtService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed sample user (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            UserName = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!")
        });
        db.SaveChanges();
    }
}

// Middleware pipeline - enable Swagger in all environments for convenience (change for production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthOnion API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

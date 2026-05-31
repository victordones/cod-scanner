using System.Text;
using cod_scanner.Data;
using cod_scanner.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var allowedOrigins = new[]
{
    "http://localhost:3000",
    "https://localhost:3000",
    "http://localhost:5173",
    "https://localhost:5173",
    "http://localhost:4200",
    "https://localhost:4200",
    "https://SEU-FRONT.vercel.app"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtKey =
    jwtSection.GetValue<string>("Key")
    ?? Environment.GetEnvironmentVariable("Jwt__Key");

var jwtIssuer =
    jwtSection.GetValue<string>("Issuer")
    ?? Environment.GetEnvironmentVariable("Jwt__Issuer");

var jwtAudience =
    jwtSection.GetValue<string>("Audience")
    ?? Environment.GetEnvironmentVariable("Jwt__Audience");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey ?? string.Empty);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseStaticFiles();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger-ui/swagger-custom.css");
    });
}

app.UseCors("AppCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
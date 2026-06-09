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

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não foi configurada.");
}

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
    "https://dashboard-cod-scanner.vercel.app/login"
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

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt__Key não foi configurada.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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
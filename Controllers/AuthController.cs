using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cod_scanner.Data;
using cod_scanner.DTOs;
using cod_scanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace cod_scanner.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IPasswordHasher<User> hasher, IConfiguration config)
    {
        _context = context;
        _hasher = hasher;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email já cadastrado" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.UserName, user.Email });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Credenciais inválidas" });

        // gerar JWT
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT Key not configured");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expiresInMinutes = jwtSection.GetValue<int?>("ExpiresInMinutes") ?? 60;

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var now = DateTime.UtcNow;
        var expiresInSeconds = (int)Math.Round((expires - now).TotalSeconds);

        var userSummary = new UserSummaryDto
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(new AuthResponseDto { Token = tokenString, ExpiresAt = expires, ExpiresIn = expiresInSeconds, User = userSummary });
    }

    [HttpPost("forgot-password")]
    public ActionResult ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        // Sempre retornar 200 para não vazar existência de conta.
        // Implementação real: gerar token de reset e enviar por e-mail.
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
            return BadRequest(new { message = "Dados inválidos" });

        // Implementação real: validar token, localizar usuário associado ao token e atualizar senha.
        // Aqui apenas retorna 200 simulando sucesso.
        return Ok();
    }
}

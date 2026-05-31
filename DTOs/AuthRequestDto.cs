using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class AuthRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;
}

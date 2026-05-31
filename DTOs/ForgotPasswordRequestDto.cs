using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class ForgotPasswordRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class ResetPasswordRequestDto
{
    [Required]
    public string Token { get; set; } = null!;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = null!;
}

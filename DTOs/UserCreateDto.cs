using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class UserCreateDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string? Name { get; set; }

    public IEnumerable<string>? Roles { get; set; }

    public string? Password { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace cod_scanner.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class UserUpdateDto
{
    [StringLength(255)]
    public string? Name { get; set; }

    public IEnumerable<string>? Roles { get; set; }
}

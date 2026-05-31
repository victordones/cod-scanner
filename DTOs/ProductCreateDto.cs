using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class ProductCreateDto
{
    [Required, StringLength(255)]
    public string Name { get; set; } = null!;

    [Required, StringLength(64)]
    public string Code { get; set; } = null!;

    [Required]
    public long PricePerKgCents { get; set; }

    public bool IsActive { get; set; } = true;
    // Extra removed — not persisted in current schema
}

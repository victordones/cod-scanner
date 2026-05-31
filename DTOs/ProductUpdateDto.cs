using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class ProductUpdateDto
{
    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(64)]
    public string? Code { get; set; }

    [Range(0, long.MaxValue)]
    public long? PricePerKgCents { get; set; }

    public bool? IsActive { get; set; }
    // Extra removed
}

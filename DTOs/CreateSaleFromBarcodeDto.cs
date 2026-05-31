using System.ComponentModel.DataAnnotations;

namespace cod_scanner.DTOs;

public class CreateSaleFromBarcodeDto
{
    [Required]
    public string ProductName { get; set; } = null!;

    [Required]
    [Range(0.000001, double.MaxValue)]
    public decimal WeightKg { get; set; }

    [Required]
    [Range(0, long.MaxValue)]
    public long TotalCents { get; set; }

    [Required]
    [Range(0, long.MaxValue)]
    public long PricePerKgCents { get; set; }

    [Required]
    public DateTime ScannedAt { get; set; }

    [Required]
    public string ProductCode { get; set; } = null!;

    [Required]
    public string Barcode { get; set; } = null!;
}

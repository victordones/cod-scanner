namespace cod_scanner.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public long PricePerKgCents { get; set; }
    public decimal? PricePerKg => PricePerKgCents / 100M;
    public bool IsActive { get; set; }
}

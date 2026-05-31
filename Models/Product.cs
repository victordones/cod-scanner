namespace cod_scanner.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal PricePerKg { get; set; }
    public bool IsActive { get; set; } = true;
}
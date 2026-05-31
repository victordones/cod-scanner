namespace cod_scanner.DTOs;

public class ProductListResponseDto
{
    public IEnumerable<ProductDto> Items { get; set; } = Array.Empty<ProductDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}

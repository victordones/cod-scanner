using cod_scanner.Data;
using cod_scanner.DTOs;
using cod_scanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cod_scanner.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ProductListResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = _context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Code.Contains(search));
        }
        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoItems = items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            PricePerKgCents = (long)Math.Round(p.PricePerKg * 100M),
            IsActive = p.IsActive
        }).ToList();

        return Ok(new ProductListResponseDto
        {
            Items = dtoItems,
            Page = page,
            PageSize = pageSize,
            Total = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required" });

        // Normalize code (simple)
        var code = dto.Code?.Trim() ?? string.Empty;

        if (await _context.Products.AnyAsync(p => p.Code == code))
            return BadRequest(new { message = "Code already exists" });

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = code,
            PricePerKg = dto.PricePerKgCents / 100M,
            IsActive = dto.IsActive
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var outDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Code = product.Code,
            PricePerKgCents = (long)Math.Round(product.PricePerKg * 100M),
            IsActive = product.IsActive
        };

        return CreatedAtAction(nameof(GetAll), new { id = outDto.Id }, outDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name))
            product.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code))
            product.Code = dto.Code.Trim();
        if (dto.PricePerKgCents.HasValue)
            product.PricePerKg = dto.PricePerKgCents.Value / 100M;
        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;

        // no Extra/UpdatedAt fields in current DB schema
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound();

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
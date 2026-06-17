using cod_scanner.Data;
using cod_scanner.DTOs;
using cod_scanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cod_scanner.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> CreateFromBarcode([FromBody] CreateSaleFromBarcodeDto dto)
    {
        // Model validation via attributes
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Ensure scannedAt saved as UTC
        var scannedUtc = dto.ScannedAt.Kind == DateTimeKind.Utc ? dto.ScannedAt : dto.ScannedAt.ToUniversalTime();

        var sale = new Sale
        {
            ProductName = dto.ProductName,
            WeightKg = dto.WeightKg,
            TotalCents = dto.TotalCents,
            PricePerKgCents = dto.PricePerKgCents,
            ScannedAt = scannedUtc,
            ProductCode = dto.ProductCode,
            Barcode = dto.Barcode
        };

        _context.Add(sale);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Sale>> GetById(Guid id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale is null) return NotFound();
        return Ok(sale);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] string? period
    )
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        var referenceDate = date?.Date ?? localNow.Date;

        DateTime startLocal;
        DateTime endLocal;

        if (string.Equals(period, "week", StringComparison.OrdinalIgnoreCase))
        {
            var daysSinceMonday = ((int)referenceDate.DayOfWeek + 6) % 7;

            startLocal = referenceDate.AddDays(-daysSinceMonday);
            endLocal = startLocal.AddDays(7);
        }
        else if (string.Equals(period, "month", StringComparison.OrdinalIgnoreCase))
        {
            startLocal = new DateTime(referenceDate.Year, referenceDate.Month, 1);
            endLocal = startLocal.AddMonths(1);
        }
        else
        {
            startLocal = referenceDate;
            endLocal = referenceDate.AddDays(1);
        }

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, timeZone);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, timeZone);

        var sales = await _context.Sales
            .AsNoTracking()
            .Where(sale => sale.ScannedAt >= startUtc && sale.ScannedAt < endUtc)
            .OrderByDescending(sale => sale.ScannedAt)
            .ToListAsync();

        return Ok(sales);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Sale>> Update(Guid id, [FromBody] CreateSaleFromBarcodeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var sale = await _context.Sales.FindAsync(id);

        if (sale is null)
            return NotFound();

        var scannedUtc = dto.ScannedAt.Kind == DateTimeKind.Utc
            ? dto.ScannedAt
            : dto.ScannedAt.ToUniversalTime();

        sale.ProductName = dto.ProductName;
        sale.WeightKg = dto.WeightKg;
        sale.TotalCents = dto.TotalCents;
        sale.PricePerKgCents = dto.PricePerKgCents;
        sale.ScannedAt = scannedUtc;
        sale.ProductCode = dto.ProductCode;
        sale.Barcode = dto.Barcode;

        await _context.SaveChangesAsync();

        return Ok(sale);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sale = await _context.Sales.FindAsync(id);

        if (sale is null)
            return NotFound();

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

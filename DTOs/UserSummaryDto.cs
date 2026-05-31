namespace cod_scanner.DTOs;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; }
}

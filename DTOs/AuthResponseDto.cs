namespace cod_scanner.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int ExpiresIn { get; set; }
    public UserSummaryDto? User { get; set; }
}

namespace PawsitiveHaven.Web.Models;

public record LoginRequest(string Username, string Password);

public record RegisterRequest(string Username, string Email, string Password);

public record AuthResponse(
    bool Success,
    string? Token,
    int? UserId,
    string? Username,
    string? UserLevel,
    string? Message
);

public class AuthState
{
    public bool IsAuthenticated { get; set; }
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? UserLevel { get; set; }

    public bool IsAdmin => UserLevel == "Admin";
}

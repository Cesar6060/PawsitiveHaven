namespace PawsitiveHaven.Api.Models.DTOs;

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

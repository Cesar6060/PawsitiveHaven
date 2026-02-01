using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}

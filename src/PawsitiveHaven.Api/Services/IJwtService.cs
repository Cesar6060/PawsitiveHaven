using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}

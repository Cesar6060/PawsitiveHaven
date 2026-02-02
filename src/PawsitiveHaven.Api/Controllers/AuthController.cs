using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("register")]
    public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request)
    {
        // Public registration is disabled - accounts are created by administrators only
        return StatusCode(403, new AuthResponse(
            false, null, null, null, null,
            "Public registration is disabled. Please contact an administrator to create an account."
        ));
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferenceDto>> GetPreferences()
    {
        var userId = GetUserId();
        var preferences = await _notificationService.GetPreferencesAsync(userId);

        if (preferences == null)
            return NotFound();

        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<NotificationPreferenceDto>> UpdatePreferences([FromBody] UpdateNotificationPreferencesRequest request)
    {
        var userId = GetUserId();
        var preferences = await _notificationService.UpdatePreferencesAsync(userId, request);

        if (preferences == null)
            return BadRequest("Failed to update preferences");

        return Ok(preferences);
    }
}

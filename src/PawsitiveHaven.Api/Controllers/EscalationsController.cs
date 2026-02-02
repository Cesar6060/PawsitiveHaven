using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EscalationsController : ControllerBase
{
    private readonly IEscalationService _escalationService;
    private readonly ILogger<EscalationsController> _logger;

    public EscalationsController(IEscalationService escalationService, ILogger<EscalationsController> logger)
    {
        _escalationService = escalationService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in claims");
        return int.Parse(userIdClaim);
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    [HttpPost]
    public async Task<ActionResult<EscalationResponse>> CreateEscalation([FromBody] CreateEscalationRequest request)
    {
        try
        {
            var userId = GetUserId();
            var escalation = await _escalationService.CreateEscalationAsync(userId, request);
            return CreatedAtAction(nameof(GetEscalation), new { id = escalation.Id }, escalation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EscalationResponse>> GetEscalation(int id)
    {
        var userId = IsAdmin() ? (int?)null : GetUserId();
        var escalation = await _escalationService.GetEscalationAsync(id, userId);

        if (escalation == null)
            return NotFound();

        return Ok(escalation);
    }

    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<EscalationListResponse>> GetPendingEscalations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _escalationService.GetPendingEscalationsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<EscalationListResponse>> GetEscalationsByStatus(
        string status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _escalationService.GetEscalationsByStatusAsync(status, page, pageSize);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<EscalationResponse>>> GetMyEscalations()
    {
        var userId = GetUserId();
        var escalations = await _escalationService.GetUserEscalationsAsync(userId);
        return Ok(escalations);
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<EscalationResponse>> UpdateEscalation(
        int id,
        [FromBody] UpdateEscalationRequest request)
    {
        try
        {
            var escalation = await _escalationService.UpdateEscalationAsync(id, request);

            if (escalation == null)
                return NotFound();

            return Ok(escalation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

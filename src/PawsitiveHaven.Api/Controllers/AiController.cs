using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        var userId = GetUserId();
        var response = await _aiService.ChatAsync(userId, request);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("generate-bio")]
    public async Task<ActionResult<object>> GenerateBio([FromBody] PetBioRequest request)
    {
        try
        {
            var bio = await _aiService.GeneratePetBioAsync(request);
            return Ok(new { success = true, bio });
        }
        catch (Exception)
        {
            return BadRequest(new { success = false, error = "Failed to generate bio" });
        }
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations()
    {
        var userId = GetUserId();
        var conversations = await _aiService.GetConversationsAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("conversations/{id}")]
    public async Task<ActionResult<ConversationDto>> GetConversation(int id)
    {
        var userId = GetUserId();
        var conversation = await _aiService.GetConversationAsync(userId, id);

        if (conversation == null)
            return NotFound();

        return Ok(conversation);
    }

    [HttpDelete("conversations/{id}")]
    public async Task<ActionResult> DeleteConversation(int id)
    {
        var userId = GetUserId();
        var success = await _aiService.DeleteConversationAsync(userId, id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}

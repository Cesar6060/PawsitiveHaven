using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly ILogger<PetsController> _logger;

    public PetsController(IPetService petService, ILogger<PetsController> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetMyPets()
    {
        var userId = GetUserId();
        var pets = await _petService.GetUserPetsAsync(userId);
        return Ok(pets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PetDto>> GetPet(int id)
    {
        var userId = GetUserId();
        var pet = await _petService.GetPetByIdAsync(id, userId);

        if (pet == null)
            return NotFound();

        return Ok(pet);
    }

    [HttpPost]
    public async Task<ActionResult<PetDto>> CreatePet([FromBody] CreatePetRequest request)
    {
        var userId = GetUserId();
        var pet = await _petService.CreatePetAsync(userId, request);

        if (pet == null)
            return BadRequest("Failed to create pet");

        return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, pet);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PetDto>> UpdatePet(int id, [FromBody] UpdatePetRequest request)
    {
        var userId = GetUserId();
        var pet = await _petService.UpdatePetAsync(id, userId, request);

        if (pet == null)
            return NotFound();

        return Ok(pet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var userId = GetUserId();
        var success = await _petService.DeletePetAsync(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    // ==================== Foster Assignment Endpoints ====================

    /// <summary>
    /// Assign a pet to a foster (Admin only)
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<PetDto>> AssignPetToFoster(int id, [FromBody] AssignPetRequest request)
    {
        _logger.LogInformation("Assigning pet {PetId} to foster {FosterId}", id, request.FosterId);

        var pet = await _petService.AssignPetToFosterAsync(id, request);

        if (pet == null)
            return BadRequest("Failed to assign pet. Pet or foster may not exist.");

        return Ok(pet);
    }

    /// <summary>
    /// Remove a pet's foster assignment (Admin only)
    /// </summary>
    [HttpPost("{id}/unassign")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<PetDto>> UnassignPet(int id)
    {
        _logger.LogInformation("Unassigning pet {PetId} from foster", id);

        var pet = await _petService.UnassignPetAsync(id);

        if (pet == null)
            return NotFound("Pet not found");

        return Ok(pet);
    }

    /// <summary>
    /// Get all pets that are not assigned to any foster (Admin only)
    /// </summary>
    [HttpGet("unassigned")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetUnassignedPets()
    {
        var pets = await _petService.GetUnassignedPetsAsync();
        return Ok(pets);
    }

    /// <summary>
    /// Get all pets with foster information (Admin only)
    /// </summary>
    [HttpGet("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetAllPets()
    {
        var pets = await _petService.GetAllPetsWithFosterAsync();
        return Ok(pets);
    }

    /// <summary>
    /// Get pets assigned to a specific foster
    /// </summary>
    [HttpGet("foster/{fosterId}")]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetPetsByFoster(int fosterId)
    {
        // Allow admins to view any foster's pets, or fosters to view their own
        var userId = GetUserId();
        if (!IsAdmin() && userId != fosterId)
        {
            return Forbid();
        }

        var pets = await _petService.GetPetsByFosterIdAsync(fosterId);
        return Ok(pets);
    }
}

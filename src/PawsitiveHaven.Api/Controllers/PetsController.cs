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

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
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
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/pets/{petId}/photos")]
[Authorize]
public class PetPhotosController : ControllerBase
{
    private readonly IPetPhotoRepository _photoRepository;
    private readonly IPetRepository _petRepository;
    private readonly IPhotoStorageService _storageService;
    private readonly ILogger<PetPhotosController> _logger;

    public PetPhotosController(
        IPetPhotoRepository photoRepository,
        IPetRepository petRepository,
        IPhotoStorageService storageService,
        ILogger<PetPhotosController> logger)
    {
        _photoRepository = photoRepository;
        _petRepository = petRepository;
        _storageService = storageService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private async Task<bool> CanAccessPet(int petId, int userId)
    {
        var pet = await _petRepository.GetByIdAsync(petId);
        if (pet == null) return false;

        // User owns the pet or is the foster
        return pet.UserId == userId || pet.FosterId == userId;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetPhotoDto>>> GetPhotos(int petId)
    {
        var userId = GetUserId();

        if (!await CanAccessPet(petId, userId))
            return NotFound("Pet not found");

        var photos = await _photoRepository.GetByPetIdAsync(petId);
        var photoDtos = photos.Select(p => MapToDto(p));

        return Ok(photoDtos);
    }

    [HttpPost]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
    public async Task<ActionResult<UploadPhotoResponse>> UploadPhoto(int petId, IFormFile file)
    {
        var userId = GetUserId();

        if (!await CanAccessPet(petId, userId))
            return NotFound(new UploadPhotoResponse(false, "Pet not found", null));

        if (file == null || file.Length == 0)
            return BadRequest(new UploadPhotoResponse(false, "No file provided", null));

        if (!_storageService.IsValidFileType(file.FileName))
            return BadRequest(new UploadPhotoResponse(false, "Invalid file type. Allowed types: jpg, jpeg, png, webp", null));

        if (!_storageService.IsValidFileSize(file.Length))
            return BadRequest(new UploadPhotoResponse(false, "File size exceeds 5MB limit", null));

        try
        {
            using var stream = file.OpenReadStream();
            var (filePath, fileName) = await _storageService.SavePhotoAsync(stream, file.FileName, petId);

            // Check if this is the first photo - make it primary
            var existingPhotos = await _photoRepository.GetByPetIdAsync(petId);
            var isPrimary = !existingPhotos.Any();

            var photo = new PetPhoto
            {
                PetId = petId,
                FileName = fileName,
                FilePath = filePath,
                IsPrimary = isPrimary,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = userId
            };

            await _photoRepository.AddAsync(photo);

            _logger.LogInformation("Photo uploaded: {PhotoId} for pet {PetId} by user {UserId}", photo.Id, petId, userId);

            return Ok(new UploadPhotoResponse(true, "Photo uploaded successfully", MapToDto(photo)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for pet {PetId}", petId);
            return StatusCode(500, new UploadPhotoResponse(false, "Error uploading photo", null));
        }
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(int petId, int photoId)
    {
        var userId = GetUserId();

        if (!await CanAccessPet(petId, userId))
            return NotFound("Pet not found");

        var photo = await _photoRepository.GetByIdAsync(photoId);

        if (photo == null || photo.PetId != petId)
            return NotFound("Photo not found");

        try
        {
            // Delete file from storage
            await _storageService.DeletePhotoAsync(photo.FilePath);

            // If this was the primary photo, set another one as primary
            var wasPrimary = photo.IsPrimary;

            // Delete from database
            await _photoRepository.DeleteAsync(photo);

            if (wasPrimary)
            {
                var remainingPhotos = await _photoRepository.GetByPetIdAsync(petId);
                var newPrimary = remainingPhotos.FirstOrDefault();
                if (newPrimary != null)
                {
                    newPrimary.IsPrimary = true;
                    await _photoRepository.UpdateAsync(newPrimary);
                }
            }

            _logger.LogInformation("Photo deleted: {PhotoId} for pet {PetId} by user {UserId}", photoId, petId, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting photo {PhotoId} for pet {PetId}", photoId, petId);
            return StatusCode(500, "Error deleting photo");
        }
    }

    [HttpPut("{photoId}/primary")]
    public async Task<ActionResult<SetPrimaryPhotoResponse>> SetPrimaryPhoto(int petId, int photoId)
    {
        var userId = GetUserId();

        if (!await CanAccessPet(petId, userId))
            return NotFound(new SetPrimaryPhotoResponse(false, "Pet not found"));

        var photo = await _photoRepository.GetByIdAsync(photoId);

        if (photo == null || photo.PetId != petId)
            return NotFound(new SetPrimaryPhotoResponse(false, "Photo not found"));

        try
        {
            // Clear existing primary flag
            await _photoRepository.ClearPrimaryFlagAsync(petId);

            // Set new primary
            photo.IsPrimary = true;
            await _photoRepository.UpdateAsync(photo);

            _logger.LogInformation("Primary photo set: {PhotoId} for pet {PetId} by user {UserId}", photoId, petId, userId);

            return Ok(new SetPrimaryPhotoResponse(true, "Primary photo updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary photo {PhotoId} for pet {PetId}", photoId, petId);
            return StatusCode(500, new SetPrimaryPhotoResponse(false, "Error setting primary photo"));
        }
    }

    private PetPhotoDto MapToDto(PetPhoto photo)
    {
        return new PetPhotoDto(
            photo.Id,
            photo.PetId,
            photo.FileName,
            photo.FilePath,
            _storageService.GetPhotoUrl(photo.FilePath),
            photo.IsPrimary,
            photo.UploadedAt,
            photo.UploadedBy
        );
    }
}

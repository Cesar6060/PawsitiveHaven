using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    /// <summary>
    /// Get all medical records for a specific pet
    /// </summary>
    [HttpGet("pets/{petId}/medical-records")]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetPetMedicalRecords(int petId)
    {
        var userId = GetUserId();
        var records = await _medicalRecordService.GetPetMedicalRecordsAsync(petId, userId);
        return Ok(records);
    }

    /// <summary>
    /// Get a specific medical record by ID
    /// </summary>
    [HttpGet("pets/{petId}/medical-records/{id}")]
    public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int petId, int id)
    {
        var userId = GetUserId();
        var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id, userId);

        if (record == null || record.PetId != petId)
            return NotFound();

        return Ok(record);
    }

    /// <summary>
    /// Create a new medical record for a pet
    /// </summary>
    [HttpPost("pets/{petId}/medical-records")]
    public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(int petId, [FromBody] CreateMedicalRecordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var record = await _medicalRecordService.CreateMedicalRecordAsync(petId, userId, request);

        if (record == null)
            return BadRequest("Failed to create medical record. Please verify you own this pet and the record type is valid.");

        return CreatedAtAction(nameof(GetMedicalRecord), new { petId, id = record.Id }, record);
    }

    /// <summary>
    /// Update an existing medical record
    /// </summary>
    [HttpPut("pets/{petId}/medical-records/{id}")]
    public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(int petId, int id, [FromBody] UpdateMedicalRecordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var record = await _medicalRecordService.UpdateMedicalRecordAsync(id, petId, userId, request);

        if (record == null)
            return NotFound();

        return Ok(record);
    }

    /// <summary>
    /// Delete a medical record
    /// </summary>
    [HttpDelete("pets/{petId}/medical-records/{id}")]
    public async Task<IActionResult> DeleteMedicalRecord(int petId, int id)
    {
        var userId = GetUserId();
        var success = await _medicalRecordService.DeleteMedicalRecordAsync(id, petId, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Get upcoming medical records with due dates (vaccinations, medications, etc.)
    /// </summary>
    [HttpGet("medical-records/upcoming")]
    public async Task<ActionResult<IEnumerable<UpcomingMedicalRecordDto>>> GetUpcomingMedicalRecords([FromQuery] int days = 30)
    {
        var userId = GetUserId();
        var records = await _medicalRecordService.GetUpcomingDueDatesAsync(userId, days);
        return Ok(records);
    }
}

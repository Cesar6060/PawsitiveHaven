using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()
    {
        var userId = GetUserId();
        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
        return Ok(appointments);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointments([FromQuery] int days = 7)
    {
        var userId = GetUserId();
        var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(userId, days);
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var userId = GetUserId();
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, userId);

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var userId = GetUserId();
        var appointment = await _appointmentService.CreateAppointmentAsync(userId, request);

        if (appointment == null)
            return BadRequest("Failed to create appointment");

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var userId = GetUserId();
        var appointment = await _appointmentService.UpdateAppointmentAsync(id, userId, request);

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<AppointmentDto>> ToggleComplete(int id)
    {
        var userId = GetUserId();
        var existing = await _appointmentService.GetAppointmentByIdAsync(id, userId);

        if (existing == null)
            return NotFound();

        var request = new UpdateAppointmentRequest(null, null, null, null, null, !existing.IsCompleted);
        var appointment = await _appointmentService.UpdateAppointmentAsync(id, userId, request);

        return Ok(appointment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var userId = GetUserId();
        var success = await _appointmentService.DeleteAppointmentAsync(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }
}

using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AppointmentDto>> GetUserAppointmentsAsync(int userId)
    {
        var appointments = await _appointmentRepository.GetByUserIdWithPetAsync(userId);
        return appointments.Select(MapToDto);
    }

    public async Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync(int userId, int days = 7)
    {
        var appointments = await _appointmentRepository.GetUpcomingByUserIdAsync(userId, days);
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id, int userId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null || appointment.UserId != userId)
            return null;

        return MapToDto(appointment);
    }

    public async Task<AppointmentDto?> CreateAppointmentAsync(int userId, CreateAppointmentRequest request)
    {
        try
        {
            var appointment = new Appointment
            {
                UserId = userId,
                PetId = request.PetId,
                Title = request.Title,
                Description = request.Description,
                AppointmentDate = request.AppointmentDate,
                AppointmentTime = request.AppointmentTime
            };

            await _appointmentRepository.AddAsync(appointment);
            _logger.LogInformation("Appointment created: {Title} for user {UserId}", appointment.Title, userId);

            return MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment for user {UserId}", userId);
            return null;
        }
    }

    public async Task<AppointmentDto?> UpdateAppointmentAsync(int id, int userId, UpdateAppointmentRequest request)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null || appointment.UserId != userId)
                return null;

            if (request.PetId.HasValue) appointment.PetId = request.PetId;
            if (request.Title != null) appointment.Title = request.Title;
            if (request.Description != null) appointment.Description = request.Description;
            if (request.AppointmentDate.HasValue) appointment.AppointmentDate = request.AppointmentDate.Value;
            if (request.AppointmentTime.HasValue) appointment.AppointmentTime = request.AppointmentTime;
            if (request.IsCompleted.HasValue) appointment.IsCompleted = request.IsCompleted.Value;

            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
            _logger.LogInformation("Appointment updated: {AppointmentId} for user {UserId}", id, userId);

            return MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {AppointmentId} for user {UserId}", id, userId);
            return null;
        }
    }

    public async Task<bool> DeleteAppointmentAsync(int id, int userId)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null || appointment.UserId != userId)
                return false;

            await _appointmentRepository.DeleteAsync(appointment);
            _logger.LogInformation("Appointment deleted: {AppointmentId} for user {UserId}", id, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {AppointmentId} for user {UserId}", id, userId);
            return false;
        }
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto(
            appointment.Id,
            appointment.UserId,
            appointment.PetId,
            appointment.Pet?.Name,
            appointment.Title,
            appointment.Description,
            appointment.AppointmentDate,
            appointment.AppointmentTime,
            appointment.IsCompleted
        );
    }
}

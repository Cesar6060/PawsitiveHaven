using PawsitiveHaven.Api.Models.DTOs;

namespace PawsitiveHaven.Api.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetUserAppointmentsAsync(int userId);
    Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync(int userId, int days = 7);
    Task<AppointmentDto?> GetAppointmentByIdAsync(int id, int userId);
    Task<AppointmentDto?> CreateAppointmentAsync(int userId, CreateAppointmentRequest request);
    Task<AppointmentDto?> UpdateAppointmentAsync(int id, int userId, UpdateAppointmentRequest request);
    Task<bool> DeleteAppointmentAsync(int id, int userId);
}

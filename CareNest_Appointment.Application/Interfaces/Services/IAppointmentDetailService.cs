using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Features.Commands.Create;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAppointmentDetailService
    {
        Task<AppointmentDetailDto> CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail);
        Task<List<AppointmentDetailDto>> GetAppointmentDetailsAsync(string appointmentId);
        Task<AppointmentDetailDashboardDto> GetDashboardAsync(string? shopId, string? fromDate, string? toDate, int? top);
    }
}

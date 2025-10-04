using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.DTOs;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAppointmentDetailService
    {
        Task<AppointmentDetailDto> CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail);
        Task<List<AppointmentDetailDto>> GetAppointmentDetailsAsync(string appointmentId);
    }
}

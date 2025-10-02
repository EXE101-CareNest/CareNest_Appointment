using CareNest_Appointment.Application.Features.Commands.Create;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAppointmentDetailService
    {
        Task CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail);
    }
}

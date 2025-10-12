using CareNest_Appointment.Application.Common;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<ResponseResult<object>> SendAppointmentConfirmationEmailAsync(string customerId, string customerName, string shopName, string appointmentId, string startTime, double totalAmount, List<object> details);
    }
}

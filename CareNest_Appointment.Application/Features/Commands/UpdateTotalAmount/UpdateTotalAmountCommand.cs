using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;

namespace CareNest_Appointment.Application.Features.Commands.UpdateTotalAmount
{
    public class UpdateTotalAmountCommand : ICommand<AppointmentResponse>
    {
        public string Id { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
    }
}

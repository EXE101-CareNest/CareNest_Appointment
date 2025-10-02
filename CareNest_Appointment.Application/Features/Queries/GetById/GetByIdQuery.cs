using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;

namespace CareNest_Appointment.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<AppointmentResponse>
    {
        public required string Id { get; set; }
    }
}

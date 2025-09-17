using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Domain.Entitites;

namespace CareNest_Appointment.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<Appointment>
    {
        public required string Id { get; set; }
    }
}

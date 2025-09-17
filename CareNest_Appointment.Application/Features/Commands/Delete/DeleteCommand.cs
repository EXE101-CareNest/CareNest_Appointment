
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;

namespace CareNest_Appointment.Application.Features.Commands.Delete
{
    public class DeleteCommand : ICommand
    {
        public required string Id { get; set; }
    }
}

using CareNest_Appointment.Application.Exceptions.Validators;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Entitites;
using Shared.Helper;

namespace CareNest_Appointment.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, Appointment>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Appointment> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            Appointment appointment = new()
            {
                Status = command.Status,
                CustomerId = command.CustomerId,
                Note = command.Note,
                PaymentMethod = command.PaymentMethod,
                StaffName = command.StaffName,
                StartTime = command.StartTime,
                TotalAmount = command.TotalAmount,
                ShopId = command.ShopId,
                BankId = command.BankId,
                BankTransactionId = command.BankTransactionId,
                IsPaid = command.IsPaid,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<Appointment>().AddAsync(appointment);
            await _unitOfWork.SaveAsync();

            return appointment;
        }
    }
}

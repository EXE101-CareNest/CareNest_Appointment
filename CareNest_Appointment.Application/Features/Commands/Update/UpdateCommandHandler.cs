using CareNest_Appointment.Domain.Entitites;
using CareNest_Appointment.Application.Exceptions;
using CareNest_Appointment.Application.Exceptions.Validators;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Commons.Constant;
using Shared.Helper;

namespace CareNest_Appointment.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, Appointment>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Appointment> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            appointment.Note = command.Note;
            appointment.Status = command.Status;
            appointment.CustomerId = command.CustomerId;
            appointment.PaymentMethod = command.PaymentMethod;
            appointment.StartTime = command.StartTime;
            appointment.StaffName = command.StaffName;
            appointment.TotalAmount = command.TotalAmount;
            appointment.Status = command.Status;
            appointment.IsPaid = command.IsPaid;
            appointment.BankId = command.BankId;
            appointment.BankTransactionId = command.BankTransactionId;
            appointment.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<Appointment>().Update(appointment);
            await _unitOfWork.SaveAsync();
            return appointment;

        }
    }
}

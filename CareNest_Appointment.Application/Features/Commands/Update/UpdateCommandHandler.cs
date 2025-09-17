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
            Appointment? order = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            order.Note = command.Note;
            order.Status = command.Status;
            order.CustomerId = command.CustomerId;
            order.PaymentMethod = command.PaymentMethod;
            order.StartTime = command.StartTime;
            order.StaffName = command.StaffName;
            order.TotalAmount = command.TotalAmount;
            order.Status = command.Status;
            order.IsPaid = command.IsPaid;
            order.BankId = command.BankId;
            order.BankTransactionId = command.BankTransactionId;
            order.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<Appointment>().Update(order);
            await _unitOfWork.SaveAsync();
            return order;

        }
    }
}

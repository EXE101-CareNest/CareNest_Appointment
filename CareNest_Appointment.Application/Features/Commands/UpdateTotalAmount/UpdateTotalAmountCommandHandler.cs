using CareNest_Appointment.Application.Exceptions;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Domain.Entitites;
using Shared.Helper;

namespace CareNest_Appointment.Application.Features.Commands.UpdateTotalAmount
{
    public class UpdateTotalAmountCommandHandler : ICommandHandler<UpdateTotalAmountCommand, AppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTotalAmountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentResponse> HandleAsync(UpdateTotalAmountCommand command)
        {
            // Kiểm tra TotalAmount phải >= 0
            if (command.TotalAmount < 0)
            {
                throw new BadRequestException("TotalAmount phải lớn hơn hoặc bằng 0");
            }

            // Tìm appointment theo Id
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            // Cập nhật TotalAmount
            appointment.TotalAmount = command.TotalAmount;
            appointment.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<Appointment>().Update(appointment);
            await _unitOfWork.SaveAsync();

            return new AppointmentResponse
            {
                Id = appointment.Id,
                CustomerId = appointment.CustomerId,
                ShopId = appointment.ShopId,
                TotalAmount = appointment.TotalAmount,
                PaymentMethod = appointment.PaymentMethod,
                Note = appointment.Note,
                Status = appointment.Status,
                StartTime = appointment.StartTime,
                StaffName = appointment.StaffName,
                BankId = appointment.BankId,
                BankTransactionId = appointment.BankTransactionId,
                IsPaid = appointment.IsPaid
            };
        }
    }
}

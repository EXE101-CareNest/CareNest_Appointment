using CareNest_Appointment.Application.Exceptions;
using CareNest_Appointment.Application.Exceptions.Validators;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Domain.Entitites;
using Microsoft.AspNetCore.Http;
using Shared.Helper;

namespace CareNest_Appointment.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, AppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShopService _shopService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateCommandHandler(IUnitOfWork unitOfWork, IShopService service, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _shopService = service;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppointmentResponse> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            //Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            //kiểm tra shop tồn tại
            var shop = await _shopService.GetShopById(command.ShopId);

            appointment.Note = command.Note;
            appointment.Status = command.Status;
            appointment.CustomerId = command.CustomerId;
            appointment.PaymentMethod = command.PaymentMethod;
            appointment.StartTime = command.StartTime;
            appointment.StaffName = command.StaffName;
            // Không cập nhật TotalAmount - giữ nguyên giá trị hiện tại
            appointment.Status = command.Status;
            appointment.IsPaid = command.IsPaid;
            appointment.BankId = command.BankId;
            appointment.BankTransactionId = command.BankTransactionId;
            appointment.UpdatedAt = TimeHelper.GetUtcNow();
            appointment.ShopId = shop.Data!.Data!.Id;
            appointment.UpdatedBy = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value;

            _unitOfWork.GetRepository<Appointment>().Update(appointment);
            await _unitOfWork.SaveAsync();
            return new AppointmentResponse
            {
                Id = appointment.Id,
                CustomerId = appointment.CustomerId,
                Note = appointment.Note,
                PaymentMethod = appointment.PaymentMethod,
                StaffName = appointment.StaffName,
                StartTime = appointment.StartTime,
                TotalAmount = appointment.TotalAmount,
                Status = appointment.Status,
                BankId = appointment.BankId,
                BankTransactionId = appointment.BankTransactionId,
                IsPaid = appointment.IsPaid,
                ShopId = shop.Data!.Data!.Id,
                ShopName = shop.Data!.Data!.Name
            };

        }
    }
}

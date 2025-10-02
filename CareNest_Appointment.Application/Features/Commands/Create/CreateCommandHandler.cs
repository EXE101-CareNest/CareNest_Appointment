using CareNest_Appointment.Application.Exceptions.Validators;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Entitites;
using Shared.Helper;

namespace CareNest_Appointment.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, AppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShopService _shopService;
        private readonly IAppointmentDetailService _appointmentDetailService;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IShopService shopService, IAppointmentDetailService appointmentDetailService)
        {
            _unitOfWork = unitOfWork;
            _shopService = shopService;
            _appointmentDetailService = appointmentDetailService;
        }

        public async Task<AppointmentResponse> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            //kiểm tra shop tồn tại
            var shop = await _shopService.GetShopById(command.ShopId);

            Appointment appointment = new()
            {
                Status = command.Status,
                CustomerId = command.CustomerId,
                Note = command.Note,
                PaymentMethod = command.PaymentMethod,
                StaffName = command.StaffName,
                StartTime = command.StartTime,
                TotalAmount = 0, // Mặc định là 0 khi tạo mới
                BankId = command.BankId,
                BankTransactionId = command.BankTransactionId,
                IsPaid = command.IsPaid,
                ShopId = shop.Data!.Data!.Id,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<Appointment>().AddAsync(appointment);
            await _unitOfWork.SaveAsync();

            // Create appointment details
            if (command.Details != null && command.Details.Any())
            {
                foreach (var detail in command.Details)
                {
                    await _appointmentDetailService.CreateAppointmentDetailAsync(appointment.Id, detail);
                }
            }

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

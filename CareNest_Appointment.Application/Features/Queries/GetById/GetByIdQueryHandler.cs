using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Domain.Entitites;

namespace CareNest_Appointment.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, AppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShopService _shopService;
        private readonly IAppointmentDetailService _appointmentDetailService;
        private readonly IAuthorizeService _authorizeService;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork, IShopService shopService, IAppointmentDetailService appointmentDetailService, IAuthorizeService authorizeService)
        {
            _unitOfWork = unitOfWork;
            _shopService = shopService;
            _appointmentDetailService = appointmentDetailService;
            _authorizeService = authorizeService;
        }

        public async Task<AppointmentResponse> HandleAsync(GetByIdQuery query)
        {
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(query.Id);

            if (appointment == null)
            {
                throw new Exception(MessageConstant.NotFound);
            }
            //kiểm tra shop tồn tại
            var shop = await _shopService.GetShopById(appointment.ShopId);
            
            //kiểm tra customer tồn tại qua authorize service
            var customer = await _authorizeService.GetAccountById(appointment.CustomerId);
            
            var response = new AppointmentResponse
            {
                Id = appointment.Id,
                CustomerId = appointment.CustomerId,
                AccountName = customer.Data!.Data!.Username,
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

            // Load appointment details
            response.Details = await _appointmentDetailService.GetAppointmentDetailsAsync(appointment.Id);

            return response;
        }
    }
}

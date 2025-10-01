using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Features.Queries.GetById;
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


        public GetByIdQueryHandler(IUnitOfWork unitOfWork, IShopService shopService)
        {
            _unitOfWork = unitOfWork;
            _shopService = shopService;
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

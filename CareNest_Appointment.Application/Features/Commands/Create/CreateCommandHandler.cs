using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Exceptions.Validators;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Entitites;
using Microsoft.AspNetCore.Http;
using Shared.Helper;
using System.Security.Claims;

namespace CareNest_Appointment.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, AppointmentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShopService _shopService;
        private readonly IAppointmentDetailService _appointmentDetailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IShopService shopService, IAppointmentDetailService appointmentDetailService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _shopService = shopService;
            _appointmentDetailService = appointmentDetailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppointmentResponse> HandleAsync(CreateCommand command)
        {
            //Validate.ValidateCreate(command);

            // Debug: Kiểm tra HttpContext và User
            var httpContext = _httpContextAccessor.HttpContext;
            Console.WriteLine($"HttpContext exists: {httpContext != null}");

            if (httpContext != null)
            {
                Console.WriteLine($"User exists: {httpContext.User != null}");
                Console.WriteLine($"User Identity exists: {httpContext.User.Identity != null}");
                Console.WriteLine($"Is Authenticated: {httpContext.User.Identity?.IsAuthenticated}");

                // Kiểm tra Authorization header
                if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    Console.WriteLine($"Authorization Header: {authHeader}");
                }

                // Lấy và in ra tất cả claims
                var claims = httpContext.User.Claims;
                Console.WriteLine("All Claims:");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                }

                var userRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userId = claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                Console.WriteLine($"Found Role: {userRole}");
                Console.WriteLine($"Found UserId: {userId}");

                // Chỉ gán CustomerId nếu role là 2 và userId có giá trị
                if (userRole == "ROLE_USER" && !string.IsNullOrEmpty(userId))
                {
                    command.CustomerId = userId;
                }
            }

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
                CreatedAt = TimeHelper.GetUtcNow(),
                CreatedBy = httpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value
            };
            await _unitOfWork.GetRepository<Appointment>().AddAsync(appointment);
            await _unitOfWork.SaveAsync();

            // Create appointment details and collect them\

            decimal totalAmount = new();
            var createdDetails = new List<AppointmentDetailDto>();
            if (command.Details != null && command.Details.Any())
            {
                foreach (var detail in command.Details)
                {
                    var createdDetail = await _appointmentDetailService.CreateAppointmentDetailAsync(appointment.Id, detail);
                    createdDetails.Add(createdDetail);
                    totalAmount += createdDetail.TotalAmount;
                }
            }
            
            appointment.TotalAmount = (double)totalAmount;
            await _unitOfWork.GetRepository<Appointment>().UpdateAsync(appointment);
            await _unitOfWork.SaveAsync();

            return new AppointmentResponse
            {
                Id = appointment.Id,
                CustomerId = appointment.CustomerId,
                Note = appointment.Note,
                PaymentMethod = appointment.PaymentMethod,
                StaffName = appointment.StaffName,
                StartTime = appointment.StartTime,
                TotalAmount = (double)totalAmount,
                Status = appointment.Status,
                BankId = appointment.BankId,
                BankTransactionId = appointment.BankTransactionId,
                IsPaid = appointment.IsPaid,
                ShopId = shop.Data!.Data!.Id,
                ShopName = shop.Data!.Data!.Name,
                Details = createdDetails
            };
        }
    }
}

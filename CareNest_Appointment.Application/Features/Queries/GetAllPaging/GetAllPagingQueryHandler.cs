using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Entitites;
using System.Linq.Expressions;

namespace CareNest_Appointment.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<AppointmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentDetailService _appointmentDetailService;
        private readonly IShopService _shopService;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork, IAppointmentDetailService appointmentDetailService, IShopService shopService)
        {
            _unitOfWork = unitOfWork;
            _appointmentDetailService = appointmentDetailService;
            _shopService = shopService;
        }

        public async Task<PageResult<AppointmentResponse>> HandleAsync(GetAllPagingQuery query)
        {
            Expression<Func<AppointmentResponse, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(query.CustomerId))
            {
                predicate = ad => ad.CustomerId.Contains(query.CustomerId);
            }
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                predicate = ad => ad.Status.Equals(query.Status);
            }
            var selector = ObjectMapperExtensions.CreateMapExpression<Appointment, AppointmentResponse>();

            var orderByFunc = GetOrderByFunc(query.SortColumn, query.SortDirection);

            var totalItems = await _unitOfWork.GetRepository<Appointment>().CountAsync(null);

            IEnumerable<AppointmentResponse> appointments = await _unitOfWork.GetRepository<Appointment>().FindAsync(
                predicate: null,
                orderBy: orderByFunc,
                selector: selector,
                pageSize: query.PageSize,
                pageIndex: query.Index);

            var appointmentsList = appointments.ToList();

            // Load appointment details for each appointment
            foreach (var appointment in appointmentsList)
            {
                if (appointment.Id != null)
                {
                    try
                    {
                        var details = await _appointmentDetailService.GetAppointmentDetailsAsync(appointment.Id);
                        appointment.Details = details;
                        Console.WriteLine($"Found {details.Count} details for appointment {appointment.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading details for appointment {appointment.Id}: {ex.Message}");
                        appointment.Details = new List<AppointmentDetailDto>();
                    }
                }
            }
            // Load shop for each appointment
            foreach (var appointment in appointmentsList)
            {
                if (appointment.Id != null)
                {
                    try
                    {
                        var details = await _shopService.GetShopById(appointment.ShopId);
                        appointment.ShopName = details.Data.Data.Name;
                        Console.WriteLine($"Shop for appointment {appointment.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading details for shop {appointment.Id}: {ex.Message}");
                        appointment.ShopName = null;
                    }
                }
            }
            return new PageResult<AppointmentResponse>(appointmentsList, totalItems, query.Index, query.PageSize);
        }


        private Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> GetOrderByFunc(string? sortColumn, string? sortDirection)
        {
            var ascending = string.IsNullOrWhiteSpace(sortDirection) || sortDirection.ToLower() != "desc";

            return sortColumn?.ToLower() switch
            {
                "updateat" => q => ascending ? q.OrderBy(a => a.UpdatedAt) : q.OrderByDescending(a => a.UpdatedAt),
                _ => q => q.OrderBy(a => a.CreatedAt)
            };
        }
    }
}

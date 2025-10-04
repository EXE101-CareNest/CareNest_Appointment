using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Domain.Entitites;

namespace CareNest_Appointment.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<AppointmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentDetailService _appointmentDetailService;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork, IAppointmentDetailService appointmentDetailService)
        {
            _unitOfWork = unitOfWork;
            _appointmentDetailService = appointmentDetailService;
        }

        public async Task<PageResult<AppointmentResponse>> HandleAsync(GetAllPagingQuery query)
        {
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

            return new PageResult<AppointmentResponse>(appointmentsList, totalItems, query.PageSize, query.Index);
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

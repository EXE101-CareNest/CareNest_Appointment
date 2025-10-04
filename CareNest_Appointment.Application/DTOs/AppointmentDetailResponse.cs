using CareNest_Appointment.Application.Common;

namespace CareNest_Appointment.Application.DTOs
{
    public class AppointmentDetailResponse
    {
        public List<AppointmentDetailDto> Items { get; set; } = new List<AppointmentDetailDto>();
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
namespace CareNest_Appointment.Application.DTOs
{
    public class AppointmentDetailPageResponse
    {
        public List<AppointmentDetailDto> Data { get; set; } = new();
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
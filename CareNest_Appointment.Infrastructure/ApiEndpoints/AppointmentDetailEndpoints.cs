namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AppointmentDetailEndpoints
    {
        public static string Create() => "/api/appointmentdetail";
        public static string GetByAppointmentIds(string? id) => $"/api/appointmentdetail?pageIndex=1&pageSize=100&sortDirection=asc&searchTerm={id}";
    }
}

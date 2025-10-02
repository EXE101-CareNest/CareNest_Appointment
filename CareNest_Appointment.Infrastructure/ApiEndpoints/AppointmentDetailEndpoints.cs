namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AppointmentDetailEndpoints
    {
        public static string Create() => "api/v1/appointment-details";
        public static string GetByAppointmentIds() => "api/v1/appointment-details/by-appointment-ids";
    }
}

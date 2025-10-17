namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AppointmentDetailEndpoints
    {
        public static string Create() => "/api/appointmentdetail";
        public static string GetByAppointmentIds(string? id) => $"/api/appointmentdetail?pageIndex=1&pageSize=100&sortDirection=asc&searchTerm={id}";
        public static string Dashboard(string? shopId, string? fromDate, string? toDate, int? top)
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(shopId)) query.Add($"shopId={shopId}");
            if (!string.IsNullOrWhiteSpace(fromDate)) query.Add($"fromDate={Uri.EscapeDataString(fromDate)}");
            if (!string.IsNullOrWhiteSpace(toDate)) query.Add($"toDate={Uri.EscapeDataString(toDate)}");
            if (top.HasValue) query.Add($"top={top.Value}");
            var q = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;
            return $"/api/appointmentdetail/dashboard{q}";
        }
    }
}

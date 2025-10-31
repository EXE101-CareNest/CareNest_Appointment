namespace CareNest_Appointment.Application.DTOs
{
    public class AppointmentDetailDashboardDto
    {
        public List<ServiceDetailStatDto> ServiceDetailStats { get; set; } = new();
        public List<ServiceStatDto> ServiceStats { get; set; } = new();
        // Optional grouping by shop
        public List<ShopStatDto>? ShopStats { get; set; }
        // Top-level enrichment when filtering by a specific shop
        public string? ShopId { get; set; }
        public string? ShopName { get; set; }

        // High-level appointment counts for dashboard
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ProcessingAppointments { get; set; }
        public int FinishedAppointments { get; set; }

        // Total revenue (sum of appointment TotalAmount) for current filter
        public double TotalRevenue { get; set; }
    }

    public class ServiceDetailStatDto
    {
        public string? ServiceDetailId { get; set; }
        public string? ServiceDetailName { get; set; }
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int Count { get; set; }
        public string? ShopId { get; set; }
        public string? ShopName { get; set; }
    }

    public class ServiceStatDto
    {
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int Count { get; set; }
        public string? ShopId { get; set; }
        public string? ShopName { get; set; }
    }

    public class ShopStatDto
    {
        public string? ShopId { get; set; }
        public string? ShopName { get; set; }
        public int TotalServiceDetailCount { get; set; }
        public int TotalServiceCount { get; set; }
        public double TotalRevenue { get; set; }
    }
}



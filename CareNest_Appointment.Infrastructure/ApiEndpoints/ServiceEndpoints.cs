namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public static class ServiceEndpoints
    {
        public static string GetByCategory(string? serviceCategoryId, int pageIndex = 1, int pageSize = 100, string sortDirection = "asc")
            => $"/api/service?pageIndex={pageIndex}&pageSize={pageSize}&sortDirection={sortDirection}&serviceCategoryId={serviceCategoryId}";

        public static string GetByCategories()
            => "/api/service/by-categories";
    }
}



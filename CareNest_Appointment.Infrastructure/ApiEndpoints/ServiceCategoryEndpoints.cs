namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public static class ServiceCategoryEndpoints
    {
        public static string Get(int pageIndex = 1, int pageSize = 100, string sortDirection = "asc", string? shopId = null)
        {
            var query = $"pageIndex={pageIndex}&pageSize={pageSize}&sortDirection={sortDirection}";
            if (!string.IsNullOrWhiteSpace(shopId))
            {
                query += $"&shopId={shopId}";
            }
            return $"/api/servicecategory?{query}";
        }
    }
}



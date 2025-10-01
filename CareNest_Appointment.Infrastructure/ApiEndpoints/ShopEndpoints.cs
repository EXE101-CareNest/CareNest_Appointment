
namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class ShopEndpoints
    {
        public static string GetById(string? id) => $"/api/shop/{id}";
    }
}

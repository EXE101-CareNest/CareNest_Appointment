namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AddressEndpoint
    {
        public static string GetById(string id) => $"/api/address/{id}";
        public static string GetDistricts(string? provinceId) => $"/api/address/district?provinceId={provinceId}";
    }
}

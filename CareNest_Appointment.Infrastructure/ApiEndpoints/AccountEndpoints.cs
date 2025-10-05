namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AccountEndpoints
    {
        public static string GetByUsername(string username) => $"/api/accounts/username/{username}";
    }
}
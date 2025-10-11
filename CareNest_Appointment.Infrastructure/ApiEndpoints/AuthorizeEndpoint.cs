namespace CareNest_Appointment.Infrastructure.ApiEndpoints
{
    public class AuthorizeEndpoint
    {
        public static string SendEmail(string accountId, string subject) => $"/email/send-mail?userId={accountId}&subject={subject}";
        public static string GetById(string? id) => $"/api/accounts/{id}";
    }
}

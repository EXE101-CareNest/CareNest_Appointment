using CareNest_Appointment.Application.DTOs;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Get account information by username
        /// </summary>
        Task<AccountDto> GetAccountByUsernameAsync(string username);

        /// <summary>
        /// Get current user's account information from JWT token
        /// </summary>
        Task<AccountDto> GetCurrentAccountAsync();
    }
}
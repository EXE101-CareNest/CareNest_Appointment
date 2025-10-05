using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Exceptions;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Infrastructure.ApiEndpoints;
using Microsoft.AspNetCore.Http;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAPIService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IAPIService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AccountDto> GetAccountByUsernameAsync(string username)
        {
            try
            {
                var result = await _apiService.GetAsync<AccountDto>("account", AccountEndpoints.GetByUsername(username));

                if (!result.IsSuccess || result.Data == null)
                {
                    throw new Exception($"Failed to get account info: {result.Message}");
                }

                return result.Data.Data!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting account by username: {ex.Message}");
            return await GetAccountByUsernameAsync(sub);
        }
    }
}
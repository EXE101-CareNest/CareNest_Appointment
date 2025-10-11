using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Domain.Commons.Base;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Infrastructure.ApiEndpoints;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IAPIService _apiService;

        public AuthorizeService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ResponseResult<AuthorizeDto>> GetAccountById(string? id)
        {
            var account = await _apiService.GetAsync<AuthorizeDto>("authorize", AuthorizeEndpoint.GetById(id));
            if (!account.IsSuccess)
            {
                throw BaseException.BadRequestBadRequestResponse("Customer Id " + MessageConstant.NotFound);
            }
            return account;
        }
    }
}

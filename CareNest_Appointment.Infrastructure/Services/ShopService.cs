using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Domain.Commons.Base;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Infrastructure.ApiEndpoints;
using Shared.Contracts;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class ShopService : IShopService
    {
        private readonly IAPIService _apiService;

        public ShopService(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ResponseResult<ShopResponse>> GetShopById(string? id)
        {
            var shop = await _apiService.GetAsync<ShopResponse>("shop", ShopEndpoints.GetById(id));
            if (!shop.IsSuccess)
            {
                throw BaseException.BadRequestBadRequestResponse("Shop Id " + MessageConstant.NotFound);
            }
            return shop;
        }
    }
}

using CareNest_Appointment.Application.Common;
using Shared.Contracts;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IShopService
    {
        Task<ResponseResult<ShopResponse>> GetShopById(string? id);

    }
}

using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IShopService
    {
        Task<ResponseResult<ShopResponse>> GetShopById(string? id);

    }
}

using CareNest_Appointment.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAPIService
    {
        Task<ResponseResult<T>> GetAsync<T>(string serviceType, string url);

        Task<ResponseResult<T>> PostAsync<T>(string url, object data);

        Task<ResponseResult<T>> PutAsync<T>(string url, object data);

        Task<ResponseResult<T>> DeleteAsync<T>(string url);

    }
}

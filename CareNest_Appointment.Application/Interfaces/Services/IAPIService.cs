using CareNest_Appointment.Application.Common;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAPIService
    {
        Task<ResponseResult<T>> GetAsync<T>(string serviceType, string url);

        Task<ResponseResult<T>> PostAsync<T>(string serviceType, string url, object data);

        Task<ResponseResult<T>> PutAsync<T>(string serviceType, string url, object data);

        Task<ResponseResult<T>> DeleteAsync<T>(string serviceType, string url);

    }
}

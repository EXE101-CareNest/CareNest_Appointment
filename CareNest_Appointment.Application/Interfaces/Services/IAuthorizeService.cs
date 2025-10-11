using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.DTOs;

namespace CareNest_Appointment.Application.Interfaces.Services
{
    public interface IAuthorizeService
    {
        Task<ResponseResult<AuthorizeDto>> GetAccountById(string? id);
    }
}

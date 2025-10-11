using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Infrastructure.ApiEndpoints;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class AppointmentDetailService : IAppointmentDetailService
    {
        private readonly IAPIService _apiService;

        public AppointmentDetailService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<AppointmentDetailDto> CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail)
        {
            var request = new
            {
                AppointmentId = appointmentId,
                detail.ServiceDetailId,
                detail.Note,
                detail.PetQuantity
            };

            var result = await _apiService.PostAsync<AppointmentDetailDto>("appointmentDetail", AppointmentDetailEndpoints.Create(), request);

            if (!result.IsSuccess || result.Data == null)
            {
                throw new Exception($"Failed to create appointment detail: {result.Message}");
            }

            return result.Data.Data!;
        }

        public async Task<List<AppointmentDetailDto>> GetAppointmentDetailsAsync(string appointmentId)
        {
            try
            {
                var result = await _apiService.GetAsync<AppointmentDetailResponse>("appointmentDetail", AppointmentDetailEndpoints.GetByAppointmentIds(appointmentId));

                if (!result.IsSuccess || result.Data == null)
                {
                    Console.WriteLine($"Failed to get appointment details: {result.Message}");
                    return new List<AppointmentDetailDto>();
                }

                return result.Data.Data?.Items ?? new List<AppointmentDetailDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting appointment details: {ex.Message}");
                return new List<AppointmentDetailDto>();
            }
        }
    }
}

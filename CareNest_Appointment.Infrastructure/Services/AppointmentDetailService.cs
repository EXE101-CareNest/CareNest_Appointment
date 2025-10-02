using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Infrastructure.ApiEndpoints;
using Shared.Contracts;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class AppointmentDetailService : IAppointmentDetailService
    {
        private readonly IAPIService _apiService;

        public AppointmentDetailService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail)
        {
            var request = new
            {
                AppointmentId = appointmentId,
                detail.ServiceDetailId,
                detail.Note,
                detail.PetQuantity
            };

            var result = await _apiService.PostAsync<object>("appointmentDetail", AppointmentDetailEndpoints.Create() ,request);

            if (!result.IsSuccess)
            {
                // Handle the error appropriately, maybe throw an exception or log it
                throw new Exception($"Failed to create appointment detail: {result.Message}");
            }
        }
    }
}

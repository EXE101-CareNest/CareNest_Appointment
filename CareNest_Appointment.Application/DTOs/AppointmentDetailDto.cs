using CareNest_Appointment.Application.Features.Commands.Create;
using System.Collections.Generic;

namespace CareNest_Appointment.Application.DTOs
{
    public class AppointmentDetailDto
    {
        public string ServiceDetailId { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int PetQuantity { get; set; }
    }
}

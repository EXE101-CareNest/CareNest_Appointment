namespace CareNest_Appointment.Application.Features.Commands.Create
{
    public class AppointmentDetailInput
    {
        public string ServiceDetailId { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int PetQuantity { get; set; }
    }
}

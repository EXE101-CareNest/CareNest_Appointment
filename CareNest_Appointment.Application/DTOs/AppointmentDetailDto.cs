namespace CareNest_Appointment.Application.DTOs
{
    public class AppointmentDetailDto
    {
        public string? Id { get; set; }
        public string? AppointmentId { get; set; }
        public string? ServiceDetailId { get; set; }
        public string? ServiceDetailName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }
        public int PetQuantity { get; set; }
    }
}

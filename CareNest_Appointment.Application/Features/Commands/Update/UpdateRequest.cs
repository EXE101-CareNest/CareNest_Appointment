using CareNest_Appointment.Domain.Commons.Enum;

namespace CareNest_Appointment.Application.Features.Commands.Update
{
    public class UpdateRequest
    {
        public string? CustomerId { get; set; }
        public string? ShopId { get; set; }

        public double TotalAmount { get; set; } // tổng tiền
        public string? PaymentMethod { get; set; } // phương thức thanh toán
        public string? Note { get; set; } // ghi chú

        // Status: Requested/Confirmed/To Visit/In Progress/Finished/Canceled
        public AppointmentStatus? Status { get; set; } // trạng thái

        public DateTime StartTime { get; set; } // giờ bắt đầu
        public string? StaffName { get; set; } // tên người thực hiện

        // Banking
        public string? BankId { get; set; }
        public string? BankTransactionId { get; set; }

        public bool IsPaid { get; set; }
    }
}

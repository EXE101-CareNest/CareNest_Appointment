using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CareNest_Appointment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentDetailController : ControllerBase
    {
        private readonly IAppointmentDetailService _appointmentDetailService;

        public AppointmentDetailController(IAppointmentDetailService appointmentDetailService)
        {
            _appointmentDetailService = appointmentDetailService;
        }

        /// <summary>
        /// Dashboard – Thống kê đặt dịch vụ. Lọc theo shopId (optional), khoảng thời gian (optional), top (optional).
        /// </summary>
        /// <param name="shopId">Id cửa hàng (optional). Nếu không truyền sẽ trả tổng hợp tất cả shop.</param>
        /// <param name="fromDate">ISO 8601 datetime (optional)</param>
        /// <param name="toDate">ISO 8601 datetime (optional)</param>
        /// <param name="top">Số lượng mục top trả về (optional, mặc định: 10)</param>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] string? shopId,
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate,
            [FromQuery] int? top)
        {
            AppointmentDetailDashboardDto result = await _appointmentDetailService.GetDashboardAsync(shopId, fromDate, toDate, top);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }
    }
}



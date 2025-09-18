
using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.Features.Commands.Delete;
using CareNest_Appointment.Application.Features.Commands.Update;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Features.Queries.GetById;
using CareNest_Appointment.Application.Interfaces.CQRS;
using CareNest_Appointment.Domain.Commons.Constant;
using CareNest_Appointment.Domain.Entitites;
using CareNest_Appointment.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace CareNest_Appointment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUseCaseDispatcher _dispatcher;

        public AppointmentController(IUseCaseDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Hiển thị toàn bộ danh sách cuộc hẹn  hiện có trong hệ thống với phân trang và sắp xếp
        /// </summary>
        /// <param name="pageIndex">trang hiện tại</param>
        /// <param name="pageSize">Số lượng phần tử trong trang</param>
        /// <param name="sortColumn">cột muốn sort: name, updateat,ownerid</param>
        /// <param name="sortDirection">cách sort asc or desc</param>
        /// <returns>Danh sách cuộc hẹn </returns>
        [HttpGet]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            var query = new GetAllPagingQuery()
            {
                Index = pageIndex,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _dispatcher.DispatchQueryAsync<GetAllPagingQuery, PageResult<AppointmentResponse>>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// Hiển thị chi tiết cuộc hẹn  theo id
        /// </summary>
        /// <param name="id">Id cuộc hẹn </param>
        /// <returns>chi tiết cuộc hẹn </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetByIdQuery() { Id = id };
            Appointment result = await _dispatcher.DispatchQueryAsync<GetByIdQuery, Appointment>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// tạo mới cuộc hẹn 
        /// </summary>
        /// <param name="command">thông tin cuộc hẹn </param>
        /// <returns>thông tin cuộc hẹn  mới tạo xog</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommand command)
        {
            Appointment result = await _dispatcher.DispatchAsync<CreateCommand, Appointment>(command);

            return this.OkResponse(result, MessageConstant.SuccessCreate);
        }

        /// <summary>
        /// Cập nhật thông tin cuộc hẹn 
        /// </summary>
        /// <param name="id">Id cuộc hẹn </param>
        /// <param name="request">các thông tin cần sửa</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRequest request)
        {

            var command = new UpdateCommand()
            {
                Id = id,
               CustomerId = request.CustomerId,
               Note = request.Note,
               PaymentMethod = request.PaymentMethod,
               BankId = request.BankId,
               BankTransactionId = request.BankTransactionId,
               IsPaid = request.IsPaid,
               StaffName = request.StaffName,
               StartTime = request.StartTime,
               ShopId = request.ShopId,
               Status = request.Status,
               TotalAmount = request.TotalAmount
            };
            Appointment result = await _dispatcher.DispatchAsync<UpdateCommand, Appointment>(command);

            return this.OkResponse(result, MessageConstant.SuccessUpdate);
        }

        /// <summary>
        /// xoá cuộc hẹn 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _dispatcher.DispatchAsync(new DeleteCommand { Id = id });
            return this.OkResponse(MessageConstant.SuccessDelete);
        }
    }
}

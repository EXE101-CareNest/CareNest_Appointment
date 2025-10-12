using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.Common.Options;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Infrastructure.ApiEndpoints;
using Microsoft.Extensions.Options;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IAPIService _apiService;
        private readonly HttpClient _httpClient;
        private readonly APIServiceOption _apiOptions;

        public EmailService(IAPIService apiService, HttpClient httpClient, IOptions<APIServiceOption> apiOptions)
        {
            _apiService = apiService;
            _httpClient = httpClient;
            _apiOptions = apiOptions.Value;
        }

        public async Task<ResponseResult<object>> SendAppointmentConfirmationEmailAsync(string customerId, string customerName, string shopName, string appointmentId, string startTime, double totalAmount, List<object> details)
        {
            try
            {
                string subject = $"Xác nhận đặt lịch tại {shopName}";
                string htmlContent = GenerateAppointmentConfirmationHtml(customerName, shopName, appointmentId, startTime, totalAmount, details);

                // Gọi authorize API trực tiếp với HttpClient - gửi HTML content trực tiếp
                var content = new StringContent(htmlContent, System.Text.Encoding.UTF8, "text/html");
                
                var endpoint = $"{_apiOptions.BaseUrlAuthorize}{AuthorizeEndpoint.SendEmail(customerId, subject)}";
                
                // Log thông tin gửi email
                Console.WriteLine($"=== EMAIL SERVICE DEBUG ===");
                Console.WriteLine($"Endpoint: {endpoint}");
                Console.WriteLine($"CustomerId: {customerId}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"HTML Content (first 200 chars): {htmlContent.Substring(0, Math.Min(200, htmlContent.Length))}...");
                Console.WriteLine($"HTML Content Length: {htmlContent.Length} characters");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");
                Console.WriteLine($"=== END EMAIL SERVICE DEBUG ===");
                
                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<object>
                    {
                        IsSuccess = true,
                        Message = "Email sent successfully"
                    };
                }
                else
                {
                    return new ResponseResult<object>
                    {
                        IsSuccess = false,
                        Message = $"Email service returned: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseResult<object>
                {
                    IsSuccess = false,
                    Message = $"Error sending email: {ex.Message}"
                };
            }
        }

        private string GenerateAppointmentConfirmationHtml(string customerName, string shopName, string appointmentId, string startTime, double totalAmount, List<object> details)
        {
            var detailsHtml = string.Join("", details.Select(d => 
            {
                // Parse AppointmentDetailDto từ object
                var detailJson = System.Text.Json.JsonSerializer.Serialize(d);
                var detailElement = System.Text.Json.JsonDocument.Parse(detailJson).RootElement;
                
                var serviceDetailName = detailElement.TryGetProperty("ServiceDetailName", out var nameProp) ? nameProp.GetString() : "Dịch vụ";
                var petQuantity = detailElement.TryGetProperty("PetQuantity", out var qtyProp) ? qtyProp.GetInt32() : 1;
                var note = detailElement.TryGetProperty("Note", out var noteProp) ? noteProp.GetString() : "Không có ghi chú";
                var totalAmount = detailElement.TryGetProperty("TotalAmount", out var amountProp) ? amountProp.GetDecimal() : 0;
                
                return $@"
                    <tr>
                        <td>{serviceDetailName}</td>
                        <td>{petQuantity}</td>
                        <td>{note}</td>
                        <td>{totalAmount:N0} VNĐ</td>
                    </tr>";
            }));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Xác nhận đặt lịch</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .info-table th, .info-table td {{ border: 1px solid #ddd; padding: 12px; text-align: left; }}
        .info-table th {{ background-color: #4CAF50; color: white; }}
        .details-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .details-table th, .details-table td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        .details-table th {{ background-color: #f2f2f2; font-weight: bold; }}
        .details-table tr:nth-child(even) {{ background-color: #f9f9f9; }}
        .total {{ font-weight: bold; font-size: 18px; color: #4CAF50; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Xác nhận đặt lịch thành công!</h1>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            
            <p>Cảm ơn bạn đã đặt lịch tại <strong>{shopName}</strong>. Chúng tôi đã nhận được yêu cầu của bạn và sẽ liên hệ lại trong thời gian sớm nhất.</p>
            
            <h3>📋 Thông tin đặt lịch:</h3>
            <table class='info-table'>
                <tr>
                    <th>Mã đặt lịch</th>
                    <td><strong>{appointmentId}</strong></td>
                </tr>
                <tr>
                    <th>Cửa hàng</th>
                    <td>{shopName}</td>
                </tr>
                <tr>
                    <th>Thời gian</th>
                    <td>{startTime}</td>
                </tr>
                <tr>
                    <th>Tổng tiền</th>
                    <td class='total'>{totalAmount:N0} VNĐ</td>
                </tr>
            </table>

            <h3>📝 Chi tiết dịch vụ:</h3>
            <table class='details-table'>
                <thead>
                    <tr>
                        <th>Dịch vụ</th>
                        <th>Số lượng thú cưng</th>
                        <th>Ghi chú</th>
                        <th>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>
                    {detailsHtml}
                </tbody>
            </table>

            <p><strong>Lưu ý:</strong> Vui lòng giữ mã đặt lịch này để tra cứu thông tin. Chúng tôi sẽ liên hệ với bạn trước thời gian hẹn để xác nhận.</p>
            
            <p>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
            
            <div class='footer'>
                <p>Trân trọng,<br>
                <strong>Đội ngũ {shopName}</strong></p>
                <p>📞 Hotline: 1900-12345 | 📧 Email: trungksdoa@9718428.brevosend.com</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
}

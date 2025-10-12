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

                var requestBody = new { html = htmlContent };

                // Gọi authorize API trực tiếp với HttpClient để xử lý response không phải JSON
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var endpoint = $"{_apiOptions.BaseUrlAuthorize}{AuthorizeEndpoint.SendEmail(customerId, subject)}";
                
                // Log thông tin gửi email
                Console.WriteLine($"=== EMAIL SERVICE DEBUG ===");
                Console.WriteLine($"Endpoint: {endpoint}");
                Console.WriteLine($"CustomerId: {customerId}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"JSON Request Body: {jsonContent}");
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
            var detailsHtml = string.Join("", details.Select(d => $"<li>{d}</li>"));

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
            <ul>
                {detailsHtml}
            </ul>

            <p><strong>Lưu ý:</strong> Vui lòng giữ mã đặt lịch này để tra cứu thông tin. Chúng tôi sẽ liên hệ với bạn trước thời gian hẹn để xác nhận.</p>
            
            <p>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
            
            <div class='footer'>
                <p>Trân trọng,<br>
                <strong>Đội ngũ {shopName}</strong></p>
                <p>📞 Hotline: 1900-xxxx | 📧 Email: support@carenest.com</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
}

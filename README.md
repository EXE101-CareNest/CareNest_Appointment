## CareNest Appointment Detail - API Documentation

### Dashboard – Thống kê đặt dịch vụ

- Phương thức: GET
- URL: `/api/appointmentdetail/dashboard`

#### Query params
- `fromDate` (optional): ISO 8601 datetime. Lọc từ thời điểm này trở đi. Ví dụ: `2025-10-13T00:00:00Z`.
- `toDate` (optional): ISO 8601 datetime. Lọc đến thời điểm này (bao gồm).
- `top` (optional, int): số lượng mục top trả về. Mặc định 10. Khuyến nghị 1–1000.
- `appointmentId` (optional, string): lọc theo 1 cuộc hẹn cụ thể.

Ghi chú:
- Thời gian dùng `CreatedAt` của `AppointmentDetail`, so sánh theo UTC.
- Nếu không truyền `fromDate`/`toDate`, trả toàn bộ lịch sử.
- Kết quả đã sắp xếp giảm dần theo `count`.

#### Response 200 (OK)
```json
{
  "success": true,
  "message": "Lấy dashboard thành công",
  "data": {
    "serviceDetailStats": [
      {
        "serviceDetailId": "902c2715bdcb4b5da2a5ce3c3206afb7",
        "serviceDetailName": "Tắm và mát xa",
        "serviceId": "be7261667b61428db5f3d620fafc0fe0",
        "serviceName": "Mát xa",
        "count": 42
      }
    ],
    "serviceStats": [
      {
        "serviceId": "be7261667b61428db5f3d620fafc0fe0",
        "serviceName": "Mát xa",
        "count": 73
      }
    ]
  }
}
```

#### Ví dụ gọi
- Lọc theo thời gian và top:
```
GET /api/appointmentdetail/dashboard?fromDate=2025-10-01T00:00:00Z&toDate=2025-10-13T23:59:59Z&top=10
```
- Lọc theo 1 `appointmentId`:
```
GET /api/appointmentdetail/dashboard?appointmentId=abcd1234&top=5
```

#### Lỗi có thể gặp
- 400 Bad Request: định dạng `fromDate`/`toDate` không hợp lệ, `top` <= 0.
- 500 Internal Server Error: lỗi ngoài ý muốn.

#### Hiệu năng
- Dữ liệu đã "denormalize" trong `AppointmentDetail`: `serviceId`, `serviceName`, `serviceDetailName` được lưu sẵn, không cần gọi sang service khác khi đọc dashboard.
- Nên giới hạn `top` phù hợp (mặc định 10).



EADME – HỆ THỐNG KIỂM SOÁT RA VÀO RFID
1. Tổng quan

Hệ thống kiểm soát ra vào sử dụng định danh UID (mô phỏng từ RFID) để:

Nhận diện người dùng

Ghi nhận thời gian ra/vào

Kiểm soát quyền truy cập

Ghi log toàn bộ hoạt động

Lưu trữ dữ liệu tập trung

Lưu ý:
Trong phạm vi hệ thống, UID được nhập từ client để mô phỏng dữ liệu từ RFID Reader.

2. Kiến trúc hệ thống
Thành phần

Client Input (Frontend đơn giản): nhập UID

Backend Server (API): xử lý logic

Database (SQL Server): lưu trữ dữ liệu

Cloud/Server: triển khai hệ thống

Sơ đồ hệ thống
User → Client Input → API Server → Database → Response
3. Luồng hoạt động

Người dùng nhập UID trên client

Client gửi UID lên API Server

Server xử lý:

Kiểm tra UID có tồn tại không

Kiểm tra quyền truy cập

Trả kết quả:

Granted (cho phép)

Denied (từ chối)

Ghi log vào database

4. Client (Frontend đơn giản)

Là một form web hoặc công cụ gửi request (Postman)

Cho phép nhập UID và gửi đến API

Ví dụ:

Ô nhập UID

Nút “Submit”

👉 Frontend chỉ đóng vai trò nhập dữ liệu, không phải trọng tâm của hệ thống

5. Thiết kế API
Endpoint
POST /api/access/check
Request
{
  "uid": "A1B2C3D4",
  "timestamp": "2026-03-18T10:00:00"
}
Response thành công
{
  "status": "success",
  "access": "granted",
  "message": "Access allowed"
}
Response thất bại
{
  "status": "error",
  "access": "denied",
  "message": "Invalid UID"
}
6. Thiết kế cơ sở dữ liệu
Bảng Users
Trường	Kiểu	Mô tả
id	int	ID
name	string	Tên
uid	string	UID (RFID)
role	string	Quyền
Bảng AccessLogs
Trường	Kiểu	Mô tả
id	int	ID
user_id	int	Người dùng
time	datetime	Thời gian
status	string	GRANTED / DENIED
7. Logic xử lý
Nhận UID → Tìm user

IF không tồn tại:
    DENIED

IF tồn tại:
    kiểm tra quyền
    IF hợp lệ:
        GRANTED
    ELSE:
        DENIED

Ghi log tất cả trường hợp
8. Bảo mật

Sử dụng HTTPS

API Key cho client

Validate dữ liệu đầu vào

Rate limiting

9. Logging

Ghi lại:

Truy cập thành công

Truy cập thất bại

Phục vụ kiểm tra và audit

10. Backup & triển khai

Backup database định kỳ

Triển khai backend trên server/cloud

SQL Server dùng làm hệ quản trị CSDL

11. Cấu trúc thư mục dự án
rfid-access-system/
│
├── README.md
├── docs/
│
├── backend/
│   ├── controllers/
│   ├── services/
│   ├── models/
│   ├── routes/
│   └── config/
│
├── database/
│   ├── migrations/
│   └── seed/
│
├── client/
│   └── simple-form/   # form nhập UID
│
└── logs/
12. Tóm tắt hệ thống
Nhập UID → Gửi server → Kiểm tra → Trả kết quả → Ghi log
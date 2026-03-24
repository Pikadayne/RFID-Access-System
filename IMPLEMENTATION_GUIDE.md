# IMPLEMENTATION GUIDE - Hướng Dẫn Chạy Hệ Thống

## 📋 Danh Sách Điều Kiện

### System Requirements
- **OS**: Windows 10/11 hoặc Windows Server 2019+
- **RAM**: Tối thiểu 4GB (khuyến nghị 8GB)
- **.NET SDK**: 8.0+ (download từ https://dotnet.microsoft.com/download)
- **SQL Server**: 2019+ (Express, Standard, hoặc Enterprise)
  - Download Express miễn phí: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **IDE**: Visual Studio 2022 Community (miễn phí) hoặc VS Code
- **Browser**: Chrome, Firefox, Edge, Safari (tùy chọn)

---

## 🚀 Bước 1: Cài Đặt Các Công Cụ

### 1.1 Cài .NET 8.0 SDK
```bash
# Kiểm tra xem đã cài chưa
dotnet --version

# Nếu chưa, download từ
https://dotnet.microsoft.com/en-us/download/dotnet/8.0
```

### 1.2 Cài SQL Server 2019 Express
```bash
# Download từ:
https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# Chọn Edition: Express
# Chọn Default instance hoặc custom tên

# Sau cài xong, kiểm tra:
# Start → SQL Server Management Studio (SSMS)
# Hoặc download SSMS: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
```

### 1.3 Cài Visual Studio 2022 Community (Tùy chọn)
```bash
# Nếu không có VS Code/VS
https://visualstudio.microsoft.com/downloads/

# Chọn: .NET Desktop Development, ASP.NET and web development
```

---

## 📂 Bước 2: Khởi Tạo Database

### 2.1 Mở SQL Server Management Studio
```
Start → SQL Server Management Studio (SSMS)
```

### 2.2 Tạo Database
```
1. Kết nối tới SQL Server instance (mặc định: localhost hoặc .\)
2. Mở file: IE101\Database\Scripts\01_CreateDatabase.sql
3. Bấm Execute (Ctrl+E) hoặc F5
4. Chờ hoàn thành
```

### 2.3 Xác Nhận Database Tạo
```sql
-- Chạy lệnh này để xác nhận
SELECT * FROM RfidAccessDB..Users;
-- Nên thấy 4 demo users
```

---

## 💻 Bước 3: Setup Backend .NET

### 3.1 Mở Project
```bash
# Cách 1: Command Line
cd C:\STUDENT\HOC_TAP\DO_AN_MON_HOC\IE101\RfidAccessSystem

# Cách 2: Visual Studio
File → Open → Chọn folder RfidAccessSystem
```

### 3.2 Cấu Hình Connection String
```bash
# Mở file: appsettings.json
# Tìm dòng "DefaultConnection"
# Cập nhật nếu cần:

(Default - Localhost)
"Server=localhost;Database=RfidAccessDB;User Id=sa;Password=YourPassword;Encrypt=false;"

(Nếu sử dụng Windows Authentication)
"Server=localhost;Database=RfidAccessDB;Integrated Security=true;Encrypt=false;"

(Nếu SQL Server đặt tên khác)
"Server=.\\SQLEXPRESS;Database=RfidAccessDB;User Id=sa;Password=YourPassword;Encrypt=false;"
```

### 3.3 Khôi Phục NuGet Packages
```bash
dotnet restore
```

### 3.4 Chạy Backend
```bash
# Terminal:
dotnet run

# Hoặc Visual Studio:
# Nhấn F5 hoặc Debug → Start Debugging

# Chờ thấy:
# Now listening on: https://localhost:5001
# Application started. Press Ctrl+C to shut down.
```

---

## 🌐 Bước 4: Truy Cập Frontend

### 4.1 Mở Browser
```
URL: https://localhost:5001
hoặc http://localhost:5000
```

### 4.2 Nếu Có Lỗi Certificate
```
Bỏ qua cảnh báo SSL certificate
VS Code: "Advanced" → "Proceed to localhost"
Chrome: "Advanced" → "Proceed to localhost"
```

---

## ✅ Bước 5: Test Hệ Thống

### 5.1 Test Access Check (Main Feature)
```
1. Truy cập: https://localhost:5001
2. Nhập UID: A1B2C3D4
3. Chọn Access Type: ENTRY
4. Bấm "✓ Check Access"
5. Kết quả: ✓ ACCESS GRANTED (xanh)
```

### 5.2 Test với UID Không Hợp Lệ
```
1. Nhập UID: INVALID
2. Chọn Access Type: ENTRY
3. Bấm "✓ Check Access"
4. Kết quả: ✗ ACCESS DENIED (đỏ)
```

### 5.3 Test User Management
```
1. Click tab "Users"
2. Xem danh sách 4 demo users
3. Bấm "+ Add User"
4. Nhập:
   - UID: TEST001
   - Full Name: Test User
   - Email: test@example.com
   - Role: Employee
   - Department: Test
5. Bấm "Save"
6. Xem user mới trong danh sách
```

### 5.4 Test Access Logs
```
1. Click tab "Logs"
2. Nên thấy ~5 sample logs
3. Filter bằng date range
```

### 5.5 Test Reports
```
1. Click tab "Reports"
2. Nên thấy:
   - Summary stats
   - Pie chart (Granted vs Denied)
   - Bar chart (Denial reasons)
   - Top 10 users
3. Thay đổi date range để test filter
```

---

## 📡 Bước 6: Test API Endpoints

### 6.1 Test với Postman
```
1. Download Postman: https://www.postman.com/downloads/
2. Tạo Request:
   - Method: POST
   - URL: https://localhost:5001/api/access/check
   - Headers:
     - X-API-Key: RFID_ABC123XYZ789
     - Content-Type: application/json
   - Body (JSON):
     {
       "uid": "A1B2C3D4",
       "accessType": "ENTRY"
     }
3. Bấm Send
4. Nên thấy response: "access": "granted"
```

### 6.2 Test với cURL (PowerShell/Command Prompt)
```bash
curl -X POST https://localhost:5001/api/access/check `
  -H "X-API-Key: RFID_ABC123XYZ789" `
  -H "Content-Type: application/json" `
  -d '{\"uid\": \"A1B2C3D4\", \"accessType\": \"ENTRY\"}' `
  -k
```

### 6.3 Get Users API
```bash
curl -X GET https://localhost:5001/api/users `
  -H "X-API-Key: RFID_ABC123XYZ789" `
  -k
```

### 6.4 Get Reports API
```bash
curl -X GET "https://localhost:5001/api/reports/summary" `
  -H "X-API-Key: RFID_ABC123XYZ789" `
  -k
```

---

## 🐛 Sửa Lỗi Thường Gặp

### Lỗi 1: "Connection to SQL Server failed"
```
Nguyên nhân: Connection string sai hoặc SQL Server không chạy

Cách sửa:
1. Kiểm tra SQL Server đang chạy:
   - Start → Services → SQL Server (MSSQLSERVER)
   - Nếu dừng → nhấp chuột phải → Start
2. Kiểm tra connection string trong appsettings.json
3. Kiểm tra đã chạy SQL script chưa
```

### Lỗi 2: "Port 5001 is already in use"
```
Nguyên nhân: Port 5001 đang được sử dụng bởi chương trình khác

Cách sửa:
1. Tìm process chiếm port:
   netstat -ano | findstr :5001
2. Kill process:
   taskkill /PID <PID> /F
3. Hoặc chạy backend trên port khác
```

### Lỗi 3: "Certificate error in browser"
```
Nguyên nhân: Localhost SSL certificate không tin cậy

Cách sửa:
1. Chrome: Click "Advanced" → "Proceed to localhost"
2. Firefox: "Advanced" → "Accept the Risk and Continue"
3. Hoặc sử dụng HTTP (http://localhost:5000)
```

### Lỗi 4: "API Key not found or invalid"
```
Nguyên nhân: Header X-API-Key không gửi hoặc sai

Cách sửa:
1. Kiểm tra header gửi: X-API-Key: RFID_ABC123XYZ789
2. Database phải có API key này
3. Kiểm tra: SELECT * FROM RfidAccessDB..ApiKeys;
```

### Lỗi 5: Frontend không load
```
Nguyên nhân: wwwroot folder không được serve

Cách sửa:
1. Kiểm tra wwwroot folder tồn tại
2. Kiểm tra index.html nằm trong wwwroot
3. Restart backend
4. Clear browser cache
```

---

## 📊 Project Structure

```
IE101/
├── RfidAccessSystem/          ← Backend (.NET)
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── Data/
│   ├── Middleware/
│   ├── wwwroot/              ← Frontend (HTML/CSS/JS)
│   │   ├── index.html
│   │   ├── users.html
│   │   ├── logs.html
│   │   ├── reports.html
│   │   ├── css/
│   │   └── js/
│   ├── appsettings.json
│   └── Program.cs
├── Database/Scripts/
│   └── 01_CreateDatabase.sql  ← SQL Script
└── (Các design docs)
```

---

## 🔧 Cấu Hình Nâng Cao

### Thay đổi API Port
```
Trong Program.cs (không cần làm nếu mặc định):
builder.WebHost.UseUrls("https://localhost:5000", "http://localhost:5001");
```

### Thay đổi Database
```
Chỉnh sửa appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "your_connection_string_here"
}
```

### Thêm CORS cho Domain Khác
```
Trong Program.cs, tìm AddCors section:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

---

## 🚢 Deployment (Optional)

### Deploy lên IIS
```
1. Publish backend:
   dotnet publish -c Release -o ./publish
2. Tạo IIS Application
3. Copy files từ publish folder
4. Cấu hình Application Pool
5. Cấu hình HTTPS certificate
```

### Deploy lên Azure
```
1. Tạo Azure App Service
2. Publish từ Visual Studio:
   Right-click Project → Publish
3. Chọn Azure App Service
4. Configure CI/CD
```

### Deploy lên Docker (Advanced)
```
Tạo Dockerfile trong project root
```

---

## 📞 Hỗ Trợ & Tài Liệu

### Documents:
- [SYSTEM_DESIGN.md](../SYSTEM_DESIGN.md) - Kiến trúc hệ thống
- [API_DOCUMENTATION.md](../API_DOCUMENTATION.md) - API Reference
- [RfidAccessSystem/README.md](../RfidAccessSystem/README.md) - Project README

### Logs:
```
Backend logs: C:\path\to\logs\api_*.txt
SQL Server logs: C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\LOG\
```

### Demo User Accounts:
| UID | Name | Password | Role |
|-----|------|----------|------|
| A1B2C3D4 | John Doe | N/A | Employee |
| E5F6G7H8 | Jane Smith | N/A | Employee |
| I9J0K1L2 | Bob Johnson | N/A | Visitor |

API Key: `RFID_ABC123XYZ789`

---

## ✨ Hoàn Tất!

Nếu tất cả các bước trên đều thành công:
- ✅ Backend chạy bình thường
- ✅ Frontend hiển thị đầy đủ
- ✅ Có thể nhập UID và kiểm tra access
- ✅ API hoạt động

→ **Hệ thống sẵn sàng sử dụng!** 🎉

---

## 📝 Ghi Chú

- Tất cả các UIDs trong database yêu cầu chứng chỉ SSL (https)
- API Key không nên chia sẻ công khai
- Nên setup firewall rules để chỉ cho phép từ client authorized
- Định kỳ backup database

Chúc bạn may mắn! 🚀

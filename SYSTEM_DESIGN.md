# HỆ THỐNG KIỂM SOÁT RA VÀO RFID - THIẾT KẾ TOÀN BỘ HỆ THỐNG
*Last Updated: March 18, 2026*

---

## 1. KIẾN TRÚC TỔNG QUÁT

```
┌─────────────────────────┐
│   CLIENT (Web Form)     │
│  - Nhập UID             │
│  - Gửi API Request      │
└────────────┬────────────┘
             │ HTTP/HTTPS
             ▼
┌─────────────────────────────────────────┐
│  BACKEND (.NET/C#)                      │
│  - API Controllers                      │
│  - Services (Logic)                     │
│  - Authentication (API Key)             │
│  - Logging & Reporting                  │
└────────────┬────────────────────────────┘
             │ SQL Connection
             ▼
┌─────────────────────────────────────────┐
│  SQL SERVER DATABASE                    │
│  - Users Table                          │
│  - Access Logs Table                    │
│  - API Keys Table                       │
│  - Reports View                         │
└─────────────────────────────────────────┘
```

---

## 2. CÔNG NGHỆ & STACK

| Thành phần | Công nghệ | Phiên bản |
|-----------|----------|---------|
| Backend | .NET 6/7/8 (C#) | Latest LTS |
| Database | SQL Server 2019+ | Express hoặc Pro |
| Frontend | HTML5 + JavaScript | Modern |
| API Format | RESTful + JSON | Standard |
| Authentication | API Key | Custom |
| Logging | Database + File | Dual |

---

## 3. THIẾT KẾ CƠSỞ DỮ LIỆU (SQL Server)

### 3.1 Bảng Users (Người dùng)
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UID NVARCHAR(50) UNIQUE NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL, -- 'Employee', 'Visitor', 'Contractor'
    Department NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
```

### 3.2 Bảng AccessLog (Lịch sử truy cập)
```sql
CREATE TABLE AccessLogs (
    LogId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    UID NVARCHAR(50) NOT NULL,
    AccessTime DATETIME DEFAULT GETDATE(),
    AccessType NVARCHAR(20) NOT NULL, -- 'ENTRY', 'EXIT'
    Status NVARCHAR(20) NOT NULL, -- 'GRANTED', 'DENIED'
    Reason NVARCHAR(255), -- Lý do nếu từ chối
    IPAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    INDEX idx_uid (UID),
    INDEX idx_time (AccessTime),
    INDEX idx_status (Status)
);
```

### 3.3 Bảng ApiKeys (Quản lý API Key)
```sql
CREATE TABLE ApiKeys (
    ApiKeyId INT PRIMARY KEY IDENTITY(1,1),
    ClientName NVARCHAR(255) NOT NULL,
    ApiKey NVARCHAR(255) UNIQUE NOT NULL,
    SecretKey NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME,
    LastUsedAt DATETIME,
    AllowedIPs NVARCHAR(500), -- "IP1,IP2,IP3"
    Description NVARCHAR(500)
);
```

### 3.4 Bảng AccessRules (Quy tắc truy cập)
```sql
CREATE TABLE AccessRules (
    RuleId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    AllowedStartTime TIME,
    AllowedEndTime TIME,
    AllowedDays NVARCHAR(100), -- "MON,TUE,WED,THU,FRI"
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
```

### 3.5 Bảng AuditLog (Audit Trail)
```sql
CREATE TABLE AuditLogs (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    Action NVARCHAR(100), -- 'CREATE', 'UPDATE', 'DELETE', 'GRANT', 'DENY'
    EntityType NVARCHAR(50), -- 'User', 'AccessLog', 'ApiKey'
    EntityId INT,
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    ChangedBy NVARCHAR(100), -- API Client hoặc Admin
    ChangedAt DATETIME DEFAULT GETDATE(),
    IP_Address NVARCHAR(50)
);
```

### 3.6 Views cho Báo Cáo

**View: vw_AccessSummary**
```sql
CREATE VIEW vw_AccessSummary AS
SELECT 
    CAST(AccessTime AS DATE) AS AccessDate,
    COUNT(*) AS TotalAccess,
    SUM(CASE WHEN Status = 'GRANTED' THEN 1 ELSE 0 END) AS GrantedCount,
    SUM(CASE WHEN Status = 'DENIED' THEN 1 ELSE 0 END) AS DeniedCount
FROM AccessLogs
GROUP BY CAST(AccessTime AS DATE);
```

**View: vw_UserAccessHistory**
```sql
CREATE VIEW vw_UserAccessHistory AS
SELECT 
    u.UserId,
    u.UID,
    u.FullName,
    u.Department,
    al.AccessTime,
    al.AccessType,
    al.Status,
    al.Reason,
    ROW_NUMBER() OVER (PARTITION BY u.UserId ORDER BY al.AccessTime DESC) AS RowNum
FROM Users u
LEFT JOIN AccessLogs al ON u.UserId = al.UserId;
```

---

## 4. THIẾT KẾ API (.NET/C#)

### 4.1 Base API Endpoints

#### Kiểm tra truy cập (Chính)
```
POST /api/access/check
Authorization: X-API-Key: {apikey}
Content-Type: application/json

Request:
{
    "uid": "A1B2C3D4",
    "accessType": "ENTRY",
    "timestamp": "2026-03-18T10:30:00Z"
}

Response (Success 200):
{
    "status": "success",
    "access": "granted",
    "message": "Access allowed",
    "userId": 1,
    "fullName": "John Doe",
    "logId": 123
}

Response (Denied 200):
{
    "status": "success",
    "access": "denied",
    "message": "User not found",
    "reason": "Invalid UID"
}

Response (Error 400/401):
{
    "status": "error",
    "message": "Invalid API Key",
    "code": "UNAUTHORIZED"
}
```

### 4.2 User Management API

#### Tạo người dùng
```
POST /api/users
Authorization: X-API-Key: {adminkey}

Request:
{
    "uid": "A1B2C3D4",
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "Employee",
    "department": "IT",
    "phoneNumber": "0917123456"
}

Response (201 Created):
{
    "status": "success",
    "userId": 1,
    "message": "User created successfully"
}
```

#### Danh sách người dùng
```
GET /api/users?page=1&pageSize=20&role=Employee&isActive=true
Authorization: X-API-Key: {apikey}

Response:
{
    "status": "success",
    "data": [
        {
            "userId": 1,
            "uid": "A1B2C3D4",
            "fullName": "John Doe",
            "email": "john@example.com",
            "role": "Employee",
            "department": "IT",
            "isActive": true,
            "createdAt": "2026-03-01T10:00:00Z"
        }
    ],
    "totalRecords": 100,
    "pageNumber": 1,
    "pageSize": 20
}
```

#### Cập nhật người dùng
```
PUT /api/users/{userId}
Authorization: X-API-Key: {apikey}

Request:
{
    "fullName": "John Smith",
    "email": "john.smith@example.com",
    "department": "HR"
}

Response (200):
{
    "status": "success",
    "message": "User updated successfully"
}
```

#### Xóa người dùng
```
DELETE /api/users/{userId}
Authorization: X-API-Key: {apikey}

Response (200):
{
    "status": "success",
    "message": "User deleted successfully"
}
```

### 4.3 Access Log API

#### Lấy lịch sử truy cập
```
GET /api/logs/access?userId={userId}&startDate=2026-03-01&endDate=2026-03-31&page=1
Authorization: X-API-Key: {apikey}

Response:
{
    "status": "success",
    "data": [
        {
            "logId": 123,
            "userId": 1,
            "fullName": "John Doe",
            "accessTime": "2026-03-18T09:30:00Z",
            "accessType": "ENTRY",
            "status": "GRANTED"
        }
    ],
    "totalRecords": 50,
    "pageNumber": 1
}
```

#### Lấy báo cáo thống kê
```
GET /api/reports/summary?startDate=2026-03-01&endDate=2026-03-31
Authorization: X-API-Key: {apikey}

Response:
{
    "status": "success",
    "data": {
        "totalAccess": 1500,
        "grantedCount": 1450,
        "deniedCount": 50,
        "uniqueUsers": 75,
        "accessByDay": [
            {
                "date": "2026-03-18",
                "totalAccess": 50,
                "granted": 48,
                "denied": 2
            }
        ],
        "deniedReasons": [
            {
                "reason": "Invalid UID",
                "count": 30
            },
            {
                "reason": "Access time unauthorized",
                "count": 20
            }
        ]
    }
}
```

### 4.4 API Key Management

#### Tạo API Key
```
POST /api/admin/apikeys
Authorization: X-API-Key: {masterkey}

Request:
{
    "clientName": "Frontend App",
    "allowedIPs": "192.168.1.10,192.168.1.20",
    "expiresAt": "2027-03-18T00:00:00Z"
}

Response:
{
    "status": "success",
    "apiKey": "RFID_ABC123XYZ789",
    "secretKey": "SECRET_XYZ789ABC123",
    "message": "Keep these keys safe!"
}
```

---

## 5. CẤU TRÚC BACKEND (.NET)

```
RfidAccessSystem/
│
├── Controllers/
│   ├── AccessController.cs          # /api/access/*
│   ├── UsersController.cs           # /api/users/*
│   ├── LogsController.cs            # /api/logs/*
│   ├── ReportsController.cs         # /api/reports/*
│   └── AdminController.cs           # /api/admin/*
│
├── Services/
│   ├── IAccessService.cs            # Interface
│   ├── AccessService.cs             # Logic kiểm tra truy cập
│   ├── IUserService.cs
│   ├── UserService.cs               # Logic người dùng
│   ├── ILogService.cs
│   ├── LogService.cs                # Logic ghi log
│   ├── IReportService.cs
│   ├── ReportService.cs             # Logic báo cáo
│   └── AuthenticationService.cs     # Xác thực API Key
│
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── AccessLog.cs
│   │   ├── ApiKey.cs
│   │   ├── AccessRule.cs
│   │   └── AuditLog.cs
│   └── DTOs/
│       ├── Request/
│       │   ├── CheckAccessRequest.cs
│       │   ├── CreateUserRequest.cs
│       │   └── UpdateUserRequest.cs
│       └── Response/
│           ├── CheckAccessResponse.cs
│           ├── UserResponse.cs
│           └── ApiResponse.cs
│
├── Data/
│   ├── ApplicationDbContext.cs      # DbContext (Entity Framework)
│   ├── Configurations/
│   │   ├── UserConfiguration.cs
│   │   └── AccessLogConfiguration.cs
│   └── Migrations/
│
├── Middleware/
│   ├── ApiKeyAuthenticationMiddleware.cs
│   ├── ExceptionHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
│
├── Utilities/
│   ├── Constants.cs
│   ├── ResponseHelper.cs
│   ├── EncryptionHelper.cs
│   └── ValidationHelper.cs
│
├── appsettings.json                 # Configuration
├── appsettings.Development.json
├── Program.cs                       # Startup
└── RfidAccessSystem.csproj
```

---

## 6. LOGIC XỬ LÝ KIỂM TRA TRUY CẬP

```csharp
// AccessService.cs - Logic kiểm tra
public class AccessService : IAccessService
{
    public async Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request)
    {
        // 1. Kiểm tra UID có tồn tại
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UID == request.UID && u.IsActive);
        
        if (user == null)
            return DeniedResponse("Invalid UID", "User not found");
        
        // 2. Kiểm tra quy tắc truy cập (thời gian, ngày)
        if (!IsAccessAllowedByRules(user.UserId, DateTime.Now))
            return DeniedResponse("Access time unauthorized", "Outside access hours");
        
        // 3. Kiểm tra trạng thái người dùng
        if (!user.IsActive)
            return DeniedResponse("User inactive", "User is not active");
        
        // 4. Ghi log truy cập thành công
        await LogAccessAttempt(user.UserId, request.UID, "GRANTED");
        
        // 5. Trả kết quả
        return SuccessResponse(user);
    }
    
    private bool IsAccessAllowedByRules(int userId, DateTime currentTime)
    {
        var rule = _context.AccessRules
            .FirstOrDefault(r => r.UserId == userId && r.IsActive);
        
        if (rule == null) return true; // Mặc định cho phép nếu không có rule
        
        // Kiểm tra giờ
        if (currentTime.TimeOfDay < rule.AllowedStartTime 
            || currentTime.TimeOfDay > rule.AllowedEndTime)
            return false;
        
        // Kiểm tra ngày
        var allowedDays = rule.AllowedDays.Split(',');
        if (!allowedDays.Contains(currentTime.DayOfWeek.ToString()))
            return false;
        
        return true;
    }
}
```

---

## 7. AUTHENTICATION - API KEY

### 7.1 Middleware Xác Thực
```csharp
public class ApiKeyAuthenticationMiddleware
{
    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        string apiKey = context.Request.Headers["X-API-Key"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Missing API Key" });
            return;
        }
        
        // Kiểm tra API Key trong database
        var key = await apiKeyService.ValidateApiKeyAsync(apiKey, context.Connection.RemoteIpAddress.ToString());
        
        if (key == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
            return;
        }
        
        context.Items["ApiKey"] = key;
        await _next(context);
    }
}
```

### 7.2 Đăng ký Middleware
```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// Dịch vụ
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();

// Middleware
var app = builder.Build();
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();
app.Run();
```

---

## 8. DASHBOARD - FRONTEND

### 8.1 Trang chính (home.html)
- **Thống kê tổng quan**: Tổng truy cập, cho phép, từ chối, hôm nay
- **Biểu đồ**: Truy cập theo giờ, theo ngày, theo người dùng
- **Danh sách truy cập gần đây**: 10 truy cập mới nhất

### 8.2 Trang Kiểm tra Truy cập (access-check.html)
- Form nhập UID
- Dropdown chọn loại (ENTRY/EXIT)
- Nút Submit
- Kết quả ngay lập tức (GRANTED/DENIED với lý do)

### 8.3 Trang Quản lý Người dùng (users.html)
- Bảng danh sách người dùng (phân trang)
- Tìm kiếm theo UID, tên, phòng ban
- Nút Thêm → Modal form
- Nút Sửa → Modal form
- Nút Xóa → Confirm dialog
- Export CSV/Excel

### 8.4 Trang Lịch sử Truy cập (access-logs.html)
- Bộ lọc: Ngày từ/đến, người dùng, trạng thái
- Bảng lịch sử với sắp xếp
- Chi tiết từng bản ghi
- Export báo cáo

### 8.5 Trang Báo cáo Thống kê (reports.html)
- Biểu đồ thống kê toàn bộ
- Báo cáo theo thời gian (ngày, tuần, tháng)
- Top người dùng truy cập
- Danh sách lý do từ chối
- Xuất báo cáo PDF/Excel

---

## 9. SECURITY IMPLEMENTATION

### 9.1 Bảo mật API
- ✅ API Key bắt buộc trên tất cả endpoints
- ✅ HTTPS/TLS cho toàn bộ giao tiếp
- ✅ IP Whitelist (nếu cần) trong ApiKey
- ✅ Rate Limiting: 100 request/phút per API Key
- ✅ Input Validation trên tất cả dữ liệu
- ✅ SQL Injection prevention (sử dụng Parameterized Queries)

### 9.2 Bảo mật Dữ liệu
- ✅ Mã hóa API Keys: Lưu hash, không plain text
- ✅ Mã hóa Connection String
- ✅ Xóa dữ liệu nhạy cảm về password/token
- ✅ Audit Trail cho mọi thay đổi

### 9.3 CORS & Headers
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

app.UseSecurityHeaders();  // Custom headers
```

---

## 10. DEPLOYMENT

### 10.1 Database Initialization
```sql
-- Chạy các migration từ Visual Studio
-- hoặc manual chạy SQL scripts
CREATE DATABASE RfidAccessDB;
USE RfidAccessDB;

-- Chạy tất cả CREATE TABLE statements ở phần 3
-- Chạy CREATE VIEW statements
-- Thêm dữ liệu demo
```

### 10.2 Backend Deployment
```bash
# Build
dotnet build

# Publish
dotnet publish -c Release -o ./publish

# Deploy lên:
# - IIS (Windows)
# - Azure App Service
# - Docker Container
# - Linux + Nginx
```

### 10.3 Frontend Deployment
```bash
# Build
npm install
npm run build  # nếu sử dụng build tool

# Deploy lên:
# - Same server (static files)
# - AWS S3 + CloudFront
# - Azure Static Web Apps
```

---

## 11. MONITORING & LOGGING

### 11.1 Logging Strategy
- **Console Logs**: Development
- **File Logs**: Production (D://Logs/access.log)
- **Database Logs**: Tất cả truy cập vào AccessLogs & AuditLogs
- **Error Logs**: Tất cả lỗi vào ErrorLogs table

### 11.2 Health Check Endpoint
```
GET /api/health

Response:
{
    "status": "healthy",
    "database": "connected",
    "timestamp": "2026-03-18T10:30:00Z"
}
```

---

## 12. TESTING

### 12.1 Unit Tests
- AccessService.CheckAccessAsync()
- UserService.CreateUser()
- ValidationLogger()

### 12.2 Integration Tests
- POST /api/access/check với UID hợp lệ → GRANTED
- POST /api/access/check với UID không hợp lệ → DENIED
- POST /api/users (không API Key) → 401
- GET /api/logs/access (valid key) → 200

### 12.3 Load Testing
- Simulate 1000+ concurrent requests
- Test DB connection pool
- Measure response time

---

## 13. TIMELINE TRIỂN KHAI

| Giai đoạn | Thời gian | Công việc |
|-----------|-----------|----------|
| **Phase 1** | Tuần 1-2 | DB Design, Entity Models, Services |
| **Phase 2** | Tuần 3-4 | API Controllers, Authentication, Tests |
| **Phase 3** | Tuần 5-6 | Frontend Dashboard, Integration |
| **Phase 4** | Tuần 7 | UAT, Bug Fixes, Documentation |
| **Phase 5** | Tuần 8 | Deployment, Training, Go-live |

---

## 14. CÓ CẤU HÌNH MẪU (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RfidAccessDB;User Id=sa;Password=YourPassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "JwtSettings": {
    "ApiKeyExpiresDays": 365
  },
  "RateLimit": {
    "RequestsPerMinute": 100
  },
  "AllowedOrigins": ["https://yourdomain.com"],
  "EnableHttps": true
}
```

---

## 15. TỔNG KẾT

✅ **Kiến trúc**: 3-tier (Frontend - Backend - Database)
✅ **Technology**: .NET C# + SQL Server
✅ **Security**: API Key + HTTPS + Rate Limit
✅ **Features**: User Mgmt + Access Log + Reports + Dashboard
✅ **Scalability**: DB indexes, async operations, Connection pooling
✅ **Maintainability**: Clean Architecture, Services layer, Unit testable

**Status**: Sẵn sàng triển khai 🚀

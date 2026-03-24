# QUICK START - IMPLEMENTATION GUIDE

## Bắt đầu nhanh chóng với .NET Backend

### Bước 1: Tạo project .NET
```bash
dotnet new webapi -n RfidAccessSystem
cd RfidAccessSystem
```

### Bước 2: Cài đặt NuGet Packages
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
```

### Bước 3: Khởi tạo Database
```sql
-- Chạy trong SQL Server Management Studio
CREATE DATABASE RfidAccessDB;
USE RfidAccessDB;

-- Sau đó chạy migration từ Visual Studio:
-- Package Manager Console: Add-Migration Initial
-- Package Manager Console: Update-Database
```

### Bước 4: File chính cần tạo
1. **Models/Entities/** - Các entity database
2. **Services/** - Business logic
3. **Controllers/** - API endpoints
4. **Data/ApplicationDbContext.cs** - EF Core context

### Bước 5: Chạy API
```bash
dotnet run
# API sẽ chạy tại https://localhost:5001 hoặc port khác
```

### Test API đơn giản
```bash
curl -X POST https://localhost:5001/api/access/check \
  -H "X-API-Key: RFID_ABC123XYZ789" \
  -H "Content-Type: application/json" \
  -d '{"uid": "A1B2C3D4", "accessType": "ENTRY"}'
```

---

## Frontend - HTML Simple
```html
<!DOCTYPE html>
<html>
<head>
    <title>RFID Access Control</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .container { max-width: 600px; margin: 0 auto; }
        input, button { padding: 10px; font-size: 16px; }
        .result { margin-top: 20px; padding: 10px; border-radius: 5px; }
        .granted { background-color: #d4edda; color: #155724; }
        .denied { background-color: #f8d7da; color: #721c24; }
    </style>
</head>
<body>
    <div class="container">
        <h1>🔐 RFID Access Control System</h1>
        
        <div style="margin: 20px 0;">
            <label>UID:</label><br>
            <input type="text" id="uid" placeholder="Enter UID" autofocus>
            
            <br><br>
            
            <label>Access Type:</label><br>
            <select id="accessType">
                <option value="ENTRY">ENTRY (Vào)</option>
                <option value="EXIT">EXIT (Ra)</option>
            </select>
            
            <br><br>
            
            <button onclick="checkAccess()">✓ Check Access</button>
        </div>
        
        <div id="result"></div>
    </div>
    
    <script>
        const API_URL = 'https://localhost:5001/api/access/check';
        const API_KEY = 'RFID_ABC123XYZ789'; // Lấy từ backend
        
        async function checkAccess() {
            const uid = document.getElementById('uid').value;
            const accessType = document.getElementById('accessType').value;
            
            if (!uid) {
                alert('Please enter UID');
                return;
            }
            
            try {
                const response = await fetch(API_URL, {
                    method: 'POST',
                    headers: {
                        'X-API-Key': API_KEY,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        uid: uid,
                        accessType: accessType,
                        timestamp: new Date().toISOString()
                    })
                });
                
                const data = await response.json();
                showResult(data);
                document.getElementById('uid').value = '';
                document.getElementById('uid').focus();
                
            } catch (error) {
                alert('Error: ' + error.message);
            }
        }
        
        function showResult(data) {
            const resultDiv = document.getElementById('result');
            const isGranted = data.access === 'granted';
            
            resultDiv.innerHTML = `
                <div class="result ${isGranted ? 'granted' : 'denied'}">
                    <strong>${isGranted ? '✓ ACCESS GRANTED' : '✗ ACCESS DENIED'}</strong>
                    <p>User: ${data.fullName || 'Unknown'}</p>
                    <p>Message: ${data.message}</p>
                    ${data.reason ? '<p>Reason: ' + data.reason + '</p>' : ''}
                </div>
            `;
        }
        
        // Nhấn Enter để kiểm tra
        document.getElementById('uid').addEventListener('keypress', function(e) {
            if (e.key === 'Enter') checkAccess();
        });
    </script>
</body>
</html>
```

---

## Database SQL - Khởi tạo
```sql
USE RfidAccessDB;

-- 1. Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UID NVARCHAR(50) UNIQUE NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    Role NVARCHAR(50) DEFAULT 'Employee',
    Department NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- 2. AccessLogs Table
CREATE TABLE AccessLogs (
    LogId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    UID NVARCHAR(50) NOT NULL,
    AccessTime DATETIME DEFAULT GETDATE(),
    AccessType NVARCHAR(20) NOT NULL, -- 'ENTRY', 'EXIT'
    Status NVARCHAR(20) NOT NULL, -- 'GRANTED', 'DENIED'
    Reason NVARCHAR(255),
    IPAddress NVARCHAR(50),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    INDEX idx_uid (UID),
    INDEX idx_time (AccessTime)
);

-- 3. ApiKeys Table
CREATE TABLE ApiKeys (
    ApiKeyId INT PRIMARY KEY IDENTITY(1,1),
    ClientName NVARCHAR(255) NOT NULL,
    ApiKey NVARCHAR(255) UNIQUE NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME
);

-- 4. AccessRules Table
CREATE TABLE AccessRules (
    RuleId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    AllowedStartTime TIME,
    AllowedEndTime TIME,
    AllowedDays NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 5. AuditLogs Table
CREATE TABLE AuditLogs (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    Action NVARCHAR(100),
    EntityType NVARCHAR(50),
    EntityId INT,
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    ChangedBy NVARCHAR(100),
    ChangedAt DATETIME DEFAULT GETDATE(),
    IP_Address NVARCHAR(50)
);

-- 6. Insert Demo Data
INSERT INTO Users (UID, FullName, Email, Role, Department, IsActive)
VALUES 
    ('A1B2C3D4', 'John Doe', 'john@example.com', 'Employee', 'IT', 1),
    ('E5F6G7H8', 'Jane Smith', 'jane@example.com', 'Employee', 'HR', 1),
    ('I9J0K1L2', 'Bob Johnson', 'bob@example.com', 'Visitor', 'Admin', 1);

-- 7. Insert Demo API Key
INSERT INTO ApiKeys (ClientName, ApiKey, IsActive)
VALUES 
    ('Frontend App', 'RFID_ABC123XYZ789', 1),
    ('Mobile App', 'RFID_DEF456QWE123', 1);

-- 8. Insert Demo Rules (9AM-5PM, Mon-Fri)
INSERT INTO AccessRules (UserId, AllowedStartTime, AllowedEndTime, AllowedDays, IsActive)
VALUES 
    (1, '09:00:00', '17:00:00', 'MON,TUE,WED,THU,FRI', 1),
    (2, '08:00:00', '18:00:00', 'MON,TUE,WED,THU,FRI', 1);
```

---

## Checklist Triển khai

- [ ] Tạo .NET project
- [ ] Cài đặt dependencies (EF Core, Serilog, etc)
- [ ] Thiết kế & tạo database
- [ ] Tạo Entity models
- [ ] Tạo DbContext
- [ ] Tạo Services (AccessService, UserService, LogService)
- [ ] Tạo Controllers (Access, Users, Reports)
- [ ] Thêm API Key middleware
- [ ] Viết unit tests
- [ ] Tạo frontend HTML đơn giản
- [ ] Cấu hình CORS
- [ ] Test API endpoints
- [ ] Deploy & cấu hình IIS/Server
- [ ] Tạo tài liệu API (Swagger/OpenAPI)

---

## Liên hệ & Hỗ trợ

Nếu có câu hỏi, xem lại **SYSTEM_DESIGN.md** để chi tiết các thành phần.

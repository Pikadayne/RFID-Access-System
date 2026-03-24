# RFID ACCESS CONTROL SYSTEM - PROJECT OVERVIEW

Ngày tạo: 18/03/2026  
Ngôn ngữ: Vietnamese/English  
Trạng thái: ✅ **READY FOR DEVELOPMENT**

---

## 📋 DOCUMENTATION STRUCTURE

### **1. README.md** *(Original Requirements)*
- Tổng quan hệ thống
- Kiến trúc cơ bản
- Luồng hoạt động
- Yêu cầu bảo mật

### **2. SYSTEM_DESIGN.md** ⭐ *(Comprehensive Design)*
**15+ sections covering:**
- 3-tier Architecture (Client - Backend - Database)
- SQL Server database schema (5 tables + views)
- .NET/C# API design with examples
- Backend project structure & code patterns
- Access control logic & validation
- API Key authentication
- Dashboard UI design (5 modules)
- Security (HTTPS, Rate Limit, Input Validation)
- Deployment strategy
- Monitoring & Logging
- Testing approach
- 8-week implementation timeline

**👉 START HERE for complete understanding**

### **3. QUICK_START.md** ⚡ *(Implementation Guide)*
- 5-step project setup
- .NET project creation
- NuGet package installation
- SQL database initialization
- HTML frontend template
- Demo data insertion
- Implementation checklist

**👉 Use this to START CODING**

### **4. API_DOCUMENTATION.md** 📡 *(API Reference)*
- Complete endpoint reference
- All 25+ endpoints documented
- Request/Response examples
- cURL, JavaScript, Python samples
- Error codes & Rate limiting
- Authentication details
- Admin operations

**👉 Use this for API INTEGRATION**

---

## 🏗️ TECHNOLOGY STACK

```
Frontend:          HTML5 + JavaScript (Simple Form + Dashboard)
Backend:           .NET 6/7/8 (C#)
Database:          SQL Server 2019+
Authentication:    API Key (X-API-Key header)
Communication:     RESTful JSON API
Deployment:        IIS / Azure App Service / Docker
```

---

## 💾 DATABASE SCHEMA

**Main Tables:**
1. **Users** - Người dùng (UID, tên, email, phòng ban, quyền)
2. **AccessLogs** - Lịch sử truy cập (UID, thời gian, trạng thái)
3. **ApiKeys** - Quản lý API Keys
4. **AccessRules** - Quy tắc truy cập (giờ, ngày)
5. **AuditLogs** - Audit trail cho tất cả thay đổi

**Views:**
- `vw_AccessSummary` - Tóm tắt truy cập
- `vw_UserAccessHistory` - Lịch sử người dùng

---

## 🔌 API ENDPOINTS SUMMARY

### Access Control
- `POST /api/access/check` - Kiểm tra truy cập ⭐

### User Management
- `POST /api/users` - Tạo người dùng
- `GET /api/users` - Danh sách người dùng
- `GET /api/users/{id}` - Chi tiết người dùng
- `PUT /api/users/{id}` - Cập nhật
- `DELETE /api/users/{id}` - Xóa

### Access Logs
- `GET /api/logs/access` - Xem lịch sử
- `GET /api/logs/access/{id}` - Chi tiết log
- `GET /api/logs/access/export` - Export CSV/Excel

### Reports
- `GET /api/reports/summary` - Tóm tắt
- `GET /api/reports/daily-stats` - Thống kê hàng ngày
- `GET /api/reports/top-users` - Top người dùng
- `GET /api/reports/denial-reasons` - Lý do từ chối

### Admin
- `POST /api/admin/apikeys` - Tạo API Key
- `GET /api/admin/apikeys` - Danh sách API Keys
- `DELETE /api/admin/apikeys/{id}` - Hủy API Key
- `GET /api/admin/audit-logs` - Xem audit trail

### System
- `GET /api/health` - Kiểm tra sức khỏe hệ thống

---

## 🔐 SECURITY FEATURES

✅ API Key Authentication (X-API-Key header)  
✅ HTTPS/TLS encryption  
✅ Rate Limiting (100 req/min per key)  
✅ IP Whitelisting (configurable)  
✅ Input validation & SQL injection prevention  
✅ Audit logging for compliance  
✅ Access rules with time-based restrictions  

---

## 📊 DASHBOARD MODULES

### 1. **Home** - Dashboard chính
- Tổng quan: Tổng truy cập, cho phép, từ chối hôm nay
- Biểu đồ: Truy cập theo giờ, ngày, người dùng
- Top 10 truy cập gần đây

### 2. **Access Check** - Kiểm tra truy cập
- Form nhập UID đơn giản
- Dropdown chọn loại (ENTRY/EXIT)
- Kết quả ngay lập tức
- Giao diện: Xanh (GRANTED), Đỏ (DENIED)

### 3. **User Management** - Quản lý người dùng
- Bảng danh sách (phân trang, tìm kiếm)
- Thêm người dùng → Modal form
- Sửa người dùng
- Xóa người dùng (confirm)
- Export CSV/Excel

### 4. **Access History** - Lịch sử truy cập
- Bộ lọc: Ngày, người dùng, trạng thái
- Bảng chi tiết có sắp xếp
- Xem chi tiết từng bản ghi
- Export báo cáo

### 5. **Reports** - Báo cáo thống kê
- Biểu đồ tổng hợp (Pie, Bar, Line)
- Báo cáo theo khoảng thời gian
- Top 20 người dùng truy cập nhiều nhất
- Danh sách lý do từ chối
- Export PDF/Excel

---

## 📅 IMPLEMENTATION TIMELINE

| Giai đoạn | Tuần | Công việc |
|-----------|------|----------|
| **I** | 1-2 | Database design, Entity models, Services |
| **II** | 3-4 | API Controllers, Auth middleware, Tests |
| **III** | 5-6 | Frontend (5 pages), Integration testing |
| **IV** | 7 | UAT, Bug fixes, Documentation |
| **V** | 8 | Deployment, Training, Go-live |

---

## ✅ KEY FEATURES

### Core Functionality
- ✅ Kiểm tra UID và cấp quyền truy cập real-time
- ✅ Lưu lịch sử truy cập chi tiết
- ✅ Quy tắc truy cập linh hoạt (giờ, ngày, người dùng)
- ✅ Báo cáo & thống kê toàn diện

### User Management
- ✅ CRUD người dùng
- ✅ Phân loại vai trò (Employee, Visitor, Contractor)
- ✅ Quản lý bộ phận (Department)
- ✅ Kích hoạt/vô hiệu hóa người dùng

### Reporting & Analytics
- ✅ Báo cáo thống kê chi tiết
- ✅ Biểu đồ trực quan
- ✅ Export CSV/Excel/PDF
- ✅ Audit trail đầy đủ

### Security & Compliance
- ✅ API Key authentication
- ✅ Rate limiting
- ✅ Audit logging
- ✅ HTTPS encryption

---

## 🚀 QUICK START STEPS

### Bước 1: Tạo .NET Project
```bash
dotnet new webapi -n RfidAccessSystem
cd RfidAccessSystem
```

### Bước 2: Cài Dependencies
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Serilog.Sinks.File
# ...xem QUICK_START.md để danh sách đầy đủ
```

### Bước 3: Khởi tạo Database
- Chạy SQL scripts từ QUICK_START.md
- Hoặc sử dụng EF Core migrations

### Bước 4: Tạo Controllers & Services
- AccessController.cs
- UserController.cs
- LogController.cs
- Các services: AccessService, UserService, LogService

### Bước 5: Deploy & Test
- Build: `dotnet build`
- Publish: `dotnet publish -c Release`
- Test API endpoints với Postman hoặc cURL

**👉 Chi tiết: Xem QUICK_START.md**

---

## 📚 FILE GUIDE

| File | Mục đích | Khi nào đọc |
|------|---------|----------|
| README.md | Yêu cầu gốc | Hiểu requirements |
| **SYSTEM_DESIGN.md** | Thiết kế chi tiết | **Bắt đầu dự án** |
| **QUICK_START.md** | Hướng dẫn code | **Bắt đầu code** |
| **API_DOCUMENTATION.md** | API reference | **Tích hợp API** |

---

## 🔍 VERIFICATION CHECKLIST

### Pre-Development
- [ ] Đọc SYSTEM_DESIGN.md hoàn toàn
- [ ] Hiểu architecture 3-tier
- [ ] Đồng ý tech stack (.NET, SQL Server, API Key)
- [ ] Review database schema

### Development
- [ ] QUICK_START.md → Tạo project
- [ ] Database → Khởi tạo tables & views
- [ ] Services → Implement business logic
- [ ] Controllers → Implement endpoints
- [ ] Frontend → HTML/JS dashboard
- [ ] Tests → Unit & Integration tests

### Deployment
- [ ] API_DOCUMENTATION.md → Test endpoints
- [ ] Security → API Keys, HTTPS, Rate Limit
- [ ] Monitoring → Logging & Audit Trail
- [ ] Backup → Database backup strategy

---

## 📞 SUPPORT & NEXT STEPS

### Nếu có câu hỏi:
1. Kiểm tra SYSTEM_DESIGN.md mục tương ứng
2. Kiểm tra QUICK_START.md hoặc API_DOCUMENTATION.md
3. Kiểm tra code examples trong các file

### Tiếp theo:
1. ✅ Setup .NET project (QUICK_START.md)
2. ✅ Khởi tạo SQL Server database
3. ✅ Implement services & controllers
4. ✅ Viết unit tests
5. ✅ Tạo frontend dashboard
6. ✅ Integration testing
7. ✅ Deploy & go-live

---

**Status: 🟢 READY FOR DEVELOPMENT**

Tất cả thiết kế, API, database schema, code structure và hướng dẫn triển khai đã sẵn sàng.

Bắt đầu với **SYSTEM_DESIGN.md** → **QUICK_START.md** → Code ✨

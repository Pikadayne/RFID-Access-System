# API DOCUMENTATION - RFID Access Control System

## Base URL
```
Production: https://api.company.com
Development: https://localhost:5001
```

## Authentication
All endpoints require `X-API-Key` header:
```
X-API-Key: YOUR_API_KEY
```

---

## 1. ACCESS CONTROL

### 1.1 Check Access (Main Endpoint)
**Endpoint:** `POST /api/access/check`

Kiểm tra xem một người dùng có được phép truy cập hay không.

**Request Headers:**
```
X-API-Key: RFID_ABC123XYZ789
Content-Type: application/json
```

**Request Body:**
```json
{
  "uid": "A1B2C3D4",
  "accessType": "ENTRY",
  "timestamp": "2026-03-18T10:30:00Z"
}
```

**Parameters:**
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| uid | string | Yes | UID từ RFID card hoặc input |
| accessType | string | Yes | 'ENTRY' hoặc 'EXIT' |
| timestamp | string | No | ISO 8601 format, mặc định now |

**Response (200 OK - Granted):**
```json
{
  "status": "success",
  "access": "granted",
  "message": "Access allowed",
  "userId": 1,
  "fullName": "John Doe",
  "department": "IT",
  "logId": 123,
  "grantedAt": "2026-03-18T10:30:00Z"
}
```

**Response (200 OK - Denied):**
```json
{
  "status": "success",
  "access": "denied",
  "message": "Access denied",
  "reason": "Invalid UID or outside access hours",
  "deniedAt": "2026-03-18T10:30:00Z"
}
```

**Response (400 Bad Request):**
```json
{
  "status": "error",
  "code": "INVALID_REQUEST",
  "message": "UID is required and must not be empty"
}
```

**Response (401 Unauthorized):**
```json
{
  "status": "error",
  "code": "UNAUTHORIZED",
  "message": "Invalid or missing API Key"
}
```

**cURL Example:**
```bash
curl -X POST https://localhost:5001/api/access/check \
  -H "X-API-Key: RFID_ABC123XYZ789" \
  -H "Content-Type: application/json" \
  -d '{
    "uid": "A1B2C3D4",
    "accessType": "ENTRY",
    "timestamp": "2026-03-18T10:30:00Z"
  }'
```

**JavaScript Example:**
```javascript
const response = await fetch('https://localhost:5001/api/access/check', {
  method: 'POST',
  headers: {
    'X-API-Key': 'RFID_ABC123XYZ789',
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    uid: 'A1B2C3D4',
    accessType: 'ENTRY',
    timestamp: new Date().toISOString()
  })
});

const data = await response.json();
console.log(data.access === 'granted' ? 'Access Allowed' : 'Access Denied');
```

**Python Example:**
```python
import requests
import json
from datetime import datetime

url = 'https://localhost:5001/api/access/check'
headers = {
    'X-API-Key': 'RFID_ABC123XYZ789',
    'Content-Type': 'application/json'
}
data = {
    'uid': 'A1B2C3D4',
    'accessType': 'ENTRY',
    'timestamp': datetime.now().isoformat() + 'Z'
}

response = requests.post(url, headers=headers, json=data)
result = response.json()
print(f"Access: {result['access']}")
```

---

## 2. USER MANAGEMENT

### 2.1 Create User
**Endpoint:** `POST /api/users`

**Request:**
```json
{
  "uid": "A1B2C3D4",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "0917123456",
  "role": "Employee",
  "department": "IT"
}
```

**Response (201 Created):**
```json
{
  "status": "success",
  "data": {
    "userId": 1,
    "uid": "A1B2C3D4",
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "Employee",
    "department": "IT",
    "isActive": true,
    "createdAt": "2026-03-18T10:00:00Z"
  }
}
```

**cURL:**
```bash
curl -X POST https://localhost:5001/api/users \
  -H "X-API-Key: RFID_ABC123XYZ789" \
  -H "Content-Type: application/json" \
  -d '{
    "uid": "A1B2C3D4",
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "Employee"
  }'
```

---

### 2.2 Get All Users
**Endpoint:** `GET /api/users`

**Query Parameters:**
```
?page=1&pageSize=20&role=Employee&isActive=true&search=john
```

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Trang (1-based) |
| pageSize | int | 20 | Số bản ghi/trang |
| role | string | - | Filter: Employee, Visitor, Contractor |
| isActive | boolean | - | Filter |
| search | string | - | Tìm theo tên hoặc UID |

**Response:**
```json
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
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalRecords": 150,
    "totalPages": 8
  }
}
```

**cURL:**
```bash
curl -X GET "https://localhost:5001/api/users?page=1&pageSize=20&role=Employee" \
  -H "X-API-Key: RFID_ABC123XYZ789"
```

---

### 2.3 Get User by ID
**Endpoint:** `GET /api/users/{userId}`

**Response:**
```json
{
  "status": "success",
  "data": {
    "userId": 1,
    "uid": "A1B2C3D4",
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "Employee",
    "department": "IT",
    "phoneNumber": "0917123456",
    "isActive": true,
    "createdAt": "2026-03-01T10:00:00Z",
    "updatedAt": "2026-03-15T14:30:00Z"
  }
}
```

---

### 2.4 Update User
**Endpoint:** `PUT /api/users/{userId}`

**Request:**
```json
{
  "fullName": "John Smith",
  "email": "john.smith@example.com",
  "department": "HR",
  "phoneNumber": "0917654321",
  "isActive": true
}
```

**Response:**
```json
{
  "status": "success",
  "message": "User updated successfully"
}
```

---

### 2.5 Delete User
**Endpoint:** `DELETE /api/users/{userId}`

**Response:**
```json
{
  "status": "success",
  "message": "User deleted successfully"
}
```

---

### 2.6 Get User's Access Rules
**Endpoint:** `GET /api/users/{userId}/access-rules`

**Response:**
```json
{
  "status": "success",
  "data": {
    "ruleId": 1,
    "userId": 1,
    "allowedStartTime": "09:00:00",
    "allowedEndTime": "17:00:00",
    "allowedDays": ["MON", "TUE", "WED", "THU", "FRI"],
    "isActive": true
  }
}
```

---

### 2.7 Update Access Rules
**Endpoint:** `PUT /api/users/{userId}/access-rules`

**Request:**
```json
{
  "allowedStartTime": "08:00:00",
  "allowedEndTime": "18:00:00",
  "allowedDays": ["MON", "TUE", "WED", "THU", "FRI"],
  "isActive": true
}
```

**Response:**
```json
{
  "status": "success",
  "message": "Access rules updated successfully"
}
```

---

## 3. ACCESS LOGS

### 3.1 Get Access Logs
**Endpoint:** `GET /api/logs/access`

**Query Parameters:**
```
?userId=1&startDate=2026-03-01&endDate=2026-03-31&status=GRANTED&page=1&pageSize=50
```

| Param | Type | Description |
|-------|------|-------------|
| userId | int | Filter theo người dùng |
| startDate | string | YYYY-MM-DD |
| endDate | string | YYYY-MM-DD |
| status | string | GRANTED hoặc DENIED |
| accessType | string | ENTRY hoặc EXIT |
| page | int | Trang (default 1) |
| pageSize | int | Số bản ghi/trang (default 50) |

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "logId": 123,
      "userId": 1,
      "uid": "A1B2C3D4",
      "fullName": "John Doe",
      "accessTime": "2026-03-18T09:30:00Z",
      "accessType": "ENTRY",
      "status": "GRANTED",
      "reason": null
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 50,
    "totalRecords": 500,
    "totalPages": 10
  }
}
```

---

### 3.2 Get Log by ID
**Endpoint:** `GET /api/logs/access/{logId}`

**Response:**
```json
{
  "status": "success",
  "data": {
    "logId": 123,
    "userId": 1,
    "uid": "A1B2C3D4",
    "fullName": "John Doe",
    "accessTime": "2026-03-18T09:30:00Z",
    "accessType": "ENTRY",
    "status": "GRANTED",
    "reason": null,
    "ipAddress": "192.168.1.10",
    "userAgent": "Chrome/90.0"
  }
}
```

---

### 3.3 Export Access Logs
**Endpoint:** `GET /api/logs/access/export`

**Query Parameters:**
```
?startDate=2026-03-01&endDate=2026-03-31&format=csv
```

**Response:** Tệp CSV/Excel được tải xuống

---

## 4. REPORTS & STATISTICS

### 4.1 Get Access Summary
**Endpoint:** `GET /api/reports/summary`

**Query Parameters:**
```
?startDate=2026-03-01&endDate=2026-03-31
```

**Response:**
```json
{
  "status": "success",
  "data": {
    "periodStart": "2026-03-01",
    "periodEnd": "2026-03-31",
    "totalAccess": 1500,
    "grantedCount": 1450,
    "deniedCount": 50,
    "uniqueUsers": 75,
    "grantedPercentage": 96.67,
    "deniedPercentage": 3.33
  }
}
```

---

### 4.2 Get Daily Statistics
**Endpoint:** `GET /api/reports/daily-stats`

**Query Parameters:**
```
?startDate=2026-03-01&endDate=2026-03-31
```

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "date": "2026-03-18",
      "totalAccess": 50,
      "grantedCount": 48,
      "deniedCount": 2,
      "uniqueUsers": 35,
      "peakHour": 9
    }
  ]
}
```

---

### 4.3 Get Top Users
**Endpoint:** `GET /api/reports/top-users`

**Query Parameters:**
```
?startDate=2026-03-01&endDate=2026-03-31&limit=10
```

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "userId": 1,
      "fullName": "John Doe",
      "uid": "A1B2C3D4",
      "accessCount": 45,
      "lastAccess": "2026-03-18T17:00:00Z"
    }
  ]
}
```

---

### 4.4 Get Denial Reasons
**Endpoint:** `GET /api/reports/denial-reasons`

**Query Parameters:**
```
?startDate=2026-03-01&endDate=2026-03-31
```

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "reason": "Invalid UID",
      "count": 30
    },
    {
      "reason": "Access time unauthorized",
      "count": 20
    },
    {
      "reason": "User inactive",
      "count": 5
    }
  ]
}
```

---

## 5. ADMIN ENDPOINTS

### 5.1 Create API Key
**Endpoint:** `POST /api/admin/apikeys`

**Request:**
```json
{
  "clientName": "Frontend App",
  "allowedIPs": "192.168.1.10,192.168.1.20",
  "expiresAt": "2027-03-18T00:00:00Z",
  "description": "For web dashboard"
}
```

**Response:**
```json
{
  "status": "success",
  "data": {
    "apiKeyId": 1,
    "apiKey": "RFID_ABC123XYZ789",
    "secretKey": "SECRET_XYZ789ABC123",
    "clientName": "Frontend App",
    "isActive": true,
    "createdAt": "2026-03-18T10:00:00Z"
  },
  "message": "Keep these keys safe! SecretKey won't be shown again."
}
```

---

### 5.2 List API Keys
**Endpoint:** `GET /api/admin/apikeys`

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "apiKeyId": 1,
      "clientName": "Frontend App",
      "apiKey": "RFID_ABC123XYZ789",
      "isActive": true,
      "createdAt": "2026-03-18T10:00:00Z",
      "lastUsedAt": "2026-03-18T15:30:00Z"
    }
  ]
}
```

---

### 5.3 Revoke API Key
**Endpoint:** `DELETE /api/admin/apikeys/{apiKeyId}`

**Response:**
```json
{
  "status": "success",
  "message": "API Key revoked successfully"
}
```

---

### 5.4 Get Audit Logs
**Endpoint:** `GET /api/admin/audit-logs`

**Query Parameters:**
```
?entityType=User&action=CREATE&startDate=2026-03-01&page=1
```

**Response:**
```json
{
  "status": "success",
  "data": [
    {
      "auditId": 1,
      "action": "CREATE",
      "entityType": "User",
      "entityId": 1,
      "oldValues": null,
      "newValues": {"fullName": "John Doe"},
      "changedBy": "admin",
      "changedAt": "2026-03-18T10:00:00Z",
      "ipAddress": "192.168.1.1"
    }
  ]
}
```

---

## 6. HEALTH CHECK

### 6.1 System Health
**Endpoint:** `GET /api/health`

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-03-18T10:30:00Z",
  "database": "connected",
  "uptime": "12:34:56",
  "version": "1.0.0"
}
```

---

## ERROR CODES

| Code | HTTP | Meaning |
|------|------|---------|
| UNAUTHORIZED | 401 | Invalid API Key |
| INVALID_REQUEST | 400 | Bad request data |
| NOT_FOUND | 404 | Resource not found |
| CONFLICT | 409 | Duplicate UID |
| INTERNAL_ERROR | 500 | Server error |
| ACCESS_DENIED | 403 | Permission denied |
| RATE_LIMIT | 429 | Too many requests |

---

## RATE LIMITING

- **Limit:** 100 requests per minute per API Key
- **Remaining:** Header `X-RateLimit-Remaining`
- **Reset:** Header `X-RateLimit-Reset`

**Example Response Headers:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 75
X-RateLimit-Reset: 1647591000
```

---

## AUTHENTICATION Best Practices

1. ✅ Luôn sử dụng HTTPS (không HTTP)
2. ✅ Bảo mật API Key - không commit vào git
3. ✅ Rotate API Keys định kỳ
4. ✅ Sử dụng Allowed IPs nếu có thể
5. ✅ Giám sát audit logs cho hành động đáng nghi

---

## EXAMPLES

### Postman Collection
```json
{
  "info": {
    "name": "RFID Access Control API",
    "version": "1.0.0"
  },
  "auth": {
    "type": "apiKey",
    "apiKey": [
      {
        "key": "value",
        "value": "RFID_ABC123XYZ789"
      },
      {
        "key": "key",
        "value": "X-API-Key"
      }
    ]
  }
}
```

### Test Cases
1. POST /api/access/check (Valid UID) → 200 GRANTED
2. POST /api/access/check (Invalid UID) → 200 DENIED
3. POST /api/access/check (No API Key) → 401
4. GET /api/users → 200 (with pagination)
5. POST /api/users + GET /api/users → Create and retrieve

---

## Support & Documentation
- For detailed system architecture, see `SYSTEM_DESIGN.md`
- For quick start, see `QUICK_START.md`
- For issues, check logs in `[LogPath]/` directory

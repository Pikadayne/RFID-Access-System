namespace RfidAccessSystem.Models.Dtos
{
    // Request DTOs
    public class CheckAccessRequest
    {
        public string UID { get; set; } = string.Empty;
        public string AccessType { get; set; } = "ENTRY"; // ENTRY, EXIT
        public DateTime? Timestamp { get; set; }
    }

    public class CreateUserRequest
    {
        public string UID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Employee";
        public string? Department { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public bool? IsActive { get; set; }
    }

    // Response DTOs
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success";
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }
    }

    public class CheckAccessResponse
    {
        public string Status { get; set; } = "success";
        public string Access { get; set; } = "granted"; // granted, denied
        public string Message { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public int? LogId { get; set; }
    }

    public class UserResponse
    {
        public int UserId { get; set; }
        public string UID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Department { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AccessLogResponse
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string UID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime AccessTime { get; set; }
        public string AccessType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class ReportResponse
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalAccess { get; set; }
        public int GrantedCount { get; set; }
        public int DeniedCount { get; set; }
        public int UniqueUsers { get; set; }
        public double GrantedPercentage { get; set; }
        public double DeniedPercentage { get; set; }
    }
}

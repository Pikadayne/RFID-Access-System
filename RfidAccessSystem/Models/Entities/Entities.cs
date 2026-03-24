using System.ComponentModel.DataAnnotations.Schema;
namespace RfidAccessSystem.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Employee"; // Employee, Visitor, Contractor
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
        public ICollection<AccessRule> AccessRules { get; set; } = new List<AccessRule>();
    }

    public class AccessLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string UID { get; set; } = string.Empty;
        public DateTime AccessTime { get; set; }
        public string AccessType { get; set; } = "ENTRY"; // ENTRY, EXIT
        public string Status { get; set; } = "GRANTED"; // GRANTED, DENIED
        public string? Reason { get; set; }
        public string? IPAddress { get; set; }

        // Foreign key
        public User? User { get; set; }
    }

    public class ApiKey
    {
        public int ApiKeyId { get; set; }
        
        public string ClientName { get; set; } = string.Empty;

        [Column("ApiKey")]
        public string KeyValue { get; set; } = string.Empty;
        public string? SecretKey { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public string? AllowedIPs { get; set; }
        public string? Description { get; set; }
    }

    public class AccessRule
    {
        public int RuleId { get; set; }
        public int UserId { get; set; }
        public TimeSpan? AllowedStartTime { get; set; }
        public TimeSpan? AllowedEndTime { get; set; }
        public string? AllowedDays { get; set; } // "MON,TUE,WED,THU,FRI"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        public User? User { get; set; }
    }

    public class AuditLog
    {
        public int AuditId { get; set; }
        public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public string EntityType { get; set; } = string.Empty; // User, AccessLog
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? IPAddress { get; set; }
    }
}

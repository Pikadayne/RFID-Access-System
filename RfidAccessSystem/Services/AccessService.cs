using Microsoft.EntityFrameworkCore;
using RfidAccessSystem.Data;
using RfidAccessSystem.Models.Entities;
using RfidAccessSystem.Models.Dtos;

namespace RfidAccessSystem.Services
{
    public interface IAccessService
    {
        Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request, string ipAddress);
    }

    public class AccessService : IAccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccessService> _logger;

        public AccessService(ApplicationDbContext context, ILogger<AccessService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request, string ipAddress)
        {
            // 1. Validate UID
            if (string.IsNullOrEmpty(request.UID))
                return DeniedResponse("Invalid input", "UID cannot be empty");

            // 2. Find user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UID == request.UID);

            if (user == null)
            {
                _logger.LogWarning($"Access denied: User not found for UID {request.UID}");
                return DeniedResponse("User not found", "Invalid UID");
            }

            // 3. Check if user is active
            if (!user.IsActive)
            {
                await LogAccessAttempt(request.UID, user.UserId, request.AccessType, "DENIED", "User is inactive", ipAddress);
                _logger.LogWarning($"Access denied: User {user.UID} is inactive");
                return DeniedResponse("User inactive", "User is not active");
            }

            // 4. Check access rules
            var rule = await _context.AccessRules
                .FirstOrDefaultAsync(r => r.UserId == user.UserId && r.IsActive);

            if (rule != null && !IsAccessAllowedByRules(rule))
            {
                await LogAccessAttempt(request.UID, user.UserId, request.AccessType, "DENIED", "Outside access hours", ipAddress);
                _logger.LogWarning($"Access denied: {user.UID} outside access hours");
                return DeniedResponse("Access time unauthorized", "You are not allowed to access at this time");
            }

            // 5. Access granted
            var logId = await LogAccessAttempt(request.UID, user.UserId, request.AccessType, "GRANTED", null, ipAddress);
            _logger.LogInformation($"Access granted to {user.UID} ({user.FullName})");

            return new CheckAccessResponse
            {
                Status = "success",
                Access = "granted",
                Message = "Access allowed",
                UserId = user.UserId,
                FullName = user.FullName,
                Department = user.Department,
                LogId = logId
            };
        }

        private bool IsAccessAllowedByRules(AccessRule rule)
        {
            var now = DateTime.Now;

            // Check time
            if (rule.AllowedStartTime.HasValue && rule.AllowedEndTime.HasValue)
            {
                if (now.TimeOfDay < rule.AllowedStartTime || now.TimeOfDay > rule.AllowedEndTime)
                    return false;
            }

            // Check day
            if (!string.IsNullOrEmpty(rule.AllowedDays))
            {
                var allowedDays = rule.AllowedDays.Split(',').Select(d => d.Trim()).ToList();
                var currentDay = now.DayOfWeek.ToString().ToUpper();
                if (!allowedDays.Contains(currentDay))
                    return false;
            }

            return true;
        }

        private async Task<int> LogAccessAttempt(string uid, int userId, string accessType, string status, string? reason, string ipAddress)
        {
            var log = new AccessLog
            {
                UID = uid,
                UserId = userId > 0 ? userId : -1,
                AccessTime = DateTime.Now,
                AccessType = accessType,
                Status = status,
                Reason = reason,
                IPAddress = ipAddress
            };

            _context.AccessLogs.Add(log);
            await _context.SaveChangesAsync();
            return log.LogId;
        }

        private static CheckAccessResponse DeniedResponse(string message, string reason)
        {
            return new CheckAccessResponse
            {
                Status = "success",
                Access = "denied",
                Message = message,
                Reason = reason
            };
        }
    }

    public interface IUserService
    {
        Task<UserResponse?> GetUserAsync(int userId);
        Task<List<UserResponse>> GetUsersAsync(int page = 1, int pageSize = 20);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse> UpdateUserAsync(int userId, UpdateUserRequest request);
        Task DeleteUserAsync(int userId);
        Task<int> GetTotalUsersAsync();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserResponse?> GetUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user == null ? null : MapToResponse(user);
        }

        public async Task<List<UserResponse>> GetUsersAsync(int page = 1, int pageSize = 20)
        {
            var users = await _context.Users
                .OrderBy(u => u.UID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToResponse).ToList();
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            // Check if UID already exists
            if (await _context.Users.AnyAsync(u => u.UID == request.UID))
                throw new Exception("UID already exists");

            var user = new User
            {
                UID = request.UID,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                Department = request.Department,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User created: {user.UID} ({user.FullName})");

            return MapToResponse(user);
        }

        public async Task<UserResponse> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.Role))
                user.Role = request.Role;
            if (!string.IsNullOrEmpty(request.Department))
                user.Department = request.Department;
            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User updated: {user.UID}");

            return MapToResponse(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User deleted: {user.UID}");
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                UID = user.UID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RfidAccessSystem.Data;
using RfidAccessSystem.Models.Dtos;

namespace RfidAccessSystem.Services
{
    public interface ILogService
    {
        Task<List<AccessLogResponse>> GetAccessLogsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50);
        Task<AccessLogResponse?> GetLogAsync(int logId);
        Task<int> GetTotalLogsAsync();
    }

    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccessLogResponse>> GetAccessLogsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            var query = _context.AccessLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(l => l.AccessTime >= startDate);

            if (endDate.HasValue)
                query = query.Where(l => l.AccessTime <= endDate.Value.AddDays(1));

            var logs = await query
                .OrderByDescending(l => l.AccessTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.User)
                .ToListAsync();

            return logs.Select(l => new AccessLogResponse
            {
                LogId = l.LogId,
                UserId = l.UserId,
                UID = l.UID,
                FullName = l.User?.FullName ?? "Unknown",
                AccessTime = l.AccessTime,
                AccessType = l.AccessType,
                Status = l.Status,
                Reason = l.Reason
            }).ToList();
        }

        public async Task<AccessLogResponse?> GetLogAsync(int logId)
        {
            var log = await _context.AccessLogs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LogId == logId);

            if (log == null)
                return null;

            return new AccessLogResponse
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UID = log.UID,
                FullName = log.User?.FullName ?? "Unknown",
                AccessTime = log.AccessTime,
                AccessType = log.AccessType,
                Status = log.Status,
                Reason = log.Reason
            };
        }

        public async Task<int> GetTotalLogsAsync()
        {
            return await _context.AccessLogs.CountAsync();
        }
    }

    public interface IReportService
    {
        Task<ReportResponse> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<dynamic>> GetDailyStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<dynamic>> GetTopUsersAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 10);
        Task<List<dynamic>> GetDenialReasonsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportResponse> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var logs = await _context.AccessLogs
                .Where(l => l.AccessTime >= startDate && l.AccessTime <= endDate)
                .ToListAsync();

            var totalAccess = logs.Count;
            var grantedCount = logs.Count(l => l.Status == "GRANTED");
            var deniedCount = logs.Count(l => l.Status == "DENIED");
            var uniqueUsers = logs.Select(l => l.UserId).Distinct().Count();

            return new ReportResponse
            {
                PeriodStart = startDate.Value,
                PeriodEnd = endDate.Value,
                TotalAccess = totalAccess,
                GrantedCount = grantedCount,
                DeniedCount = deniedCount,
                UniqueUsers = uniqueUsers,
                GrantedPercentage = totalAccess > 0 ? Math.Round((double)grantedCount / totalAccess * 100, 2) : 0,
                DeniedPercentage = totalAccess > 0 ? Math.Round((double)deniedCount / totalAccess * 100, 2) : 0
            };
        }

        public async Task<List<dynamic>> GetDailyStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var logs = await _context.AccessLogs
                .Where(l => l.AccessTime >= startDate && l.AccessTime <= endDate)
                .ToListAsync();

            var dailyStats = logs
                .GroupBy(l => l.AccessTime.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    totalAccess = g.Count(),
                    grantedCount = g.Count(l => l.Status == "GRANTED"),
                    deniedCount = g.Count(l => l.Status == "DENIED"),
                    uniqueUsers = g.Select(l => l.UserId).Distinct().Count()
                })
                .OrderBy(d => d.date)
                .Cast<dynamic>()
                .ToList();

            return dailyStats;
        }

        public async Task<List<dynamic>> GetTopUsersAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 10)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var topUsers = await _context.AccessLogs
                .Where(l => l.AccessTime >= startDate && l.AccessTime <= endDate && l.UserId > 0)
                .Include(l => l.User)
                .GroupBy(l => new { l.UserId, l.User!.UID, l.User.FullName })
                .Select(g => new
                {
                    userId = g.Key.UserId,
                    uid = g.Key.UID,
                    fullName = g.Key.FullName,
                    accessCount = g.Count(),
                    lastAccess = g.Max(l => l.AccessTime)
                })
                .OrderByDescending(u => u.accessCount)
                .Take(limit)
                .Cast<dynamic>()
                .ToListAsync();

            return topUsers;
        }

        public async Task<List<dynamic>> GetDenialReasonsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var reasons = await _context.AccessLogs
                .Where(l => l.AccessTime >= startDate && l.AccessTime <= endDate && l.Status == "DENIED" && l.Reason != null)
                .GroupBy(l => l.Reason)
                .Select(g => new
                {
                    reason = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(r => r.count)
                .Cast<dynamic>()
                .ToListAsync();

            return reasons;
        }
    }
}

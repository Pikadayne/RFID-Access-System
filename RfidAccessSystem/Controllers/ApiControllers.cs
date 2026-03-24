using Microsoft.AspNetCore.Mvc;
using RfidAccessSystem.Models.Dtos;
using RfidAccessSystem.Services;

namespace RfidAccessSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;
        private readonly ILogger<AccessController> _logger;

        public AccessController(IAccessService accessService, ILogger<AccessController> logger)
        {
            _accessService = accessService;
            _logger = logger;
        }

        /// <summary>
        /// Check user access - Main endpoint
        /// </summary>
        [HttpPost("check")]
        public async Task<IActionResult> CheckAccess([FromBody] CheckAccessRequest request)
        {
            if (request == null)
                return BadRequest(new { status = "error", message = "Request body is required" });

            if (string.IsNullOrEmpty(request.UID))
                return BadRequest(new { status = "error", message = "UID is required" });

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var response = await _accessService.CheckAccessAsync(request, ipAddress);
            
            return Ok(response);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var users = await _userService.GetUsersAsync(page, pageSize);
            var total = await _userService.GetTotalUsersAsync();

            return Ok(new
            {
                status = "success",
                data = users,
                pagination = new
                {
                    pageNumber = page,
                    pageSize = pageSize,
                    totalRecords = total,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            });
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound(new { status = "error", message = "User not found" });

            return Ok(new { status = "success", data = user });
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new { status = "success", data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request);
                return Ok(new { status = "success", data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { status = "success", message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Get access logs with filters
        /// </summary>
        [HttpGet("access")]
        public async Task<IActionResult> GetAccessLogs(
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            var logs = await _logService.GetAccessLogsAsync(userId, startDate, endDate, page, pageSize);
            var total = await _logService.GetTotalLogsAsync();

            return Ok(new
            {
                status = "success",
                data = logs,
                pagination = new
                {
                    pageNumber = page,
                    pageSize = pageSize,
                    totalRecords = total,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            });
        }

        /// <summary>
        /// Get single log
        /// </summary>
        [HttpGet("access/{id}")]
        public async Task<IActionResult> GetLog(int id)
        {
            var log = await _logService.GetLogAsync(id);
            if (log == null)
                return NotFound(new { status = "error", message = "Log not found" });

            return Ok(new { status = "success", data = log });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Get summary report
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var summary = await _reportService.GetSummaryAsync(startDate, endDate);
            return Ok(new { status = "success", data = summary });
        }

        /// <summary>
        /// Get daily statistics
        /// </summary>
        [HttpGet("daily-stats")]
        public async Task<IActionResult> GetDailyStats(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var stats = await _reportService.GetDailyStatsAsync(startDate, endDate);
            return Ok(new { status = "success", data = stats });
        }

        /// <summary>
        /// Get top users
        /// </summary>
        [HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int limit = 10)
        {
            var topUsers = await _reportService.GetTopUsersAsync(startDate, endDate, limit);
            return Ok(new { status = "success", data = topUsers });
        }

        /// <summary>
        /// Get denial reasons
        /// </summary>
        [HttpGet("denial-reasons")]
        public async Task<IActionResult> GetDenialReasons(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var reasons = await _reportService.GetDenialReasonsAsync(startDate, endDate);
            return Ok(new { status = "success", data = reasons });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
    }
}

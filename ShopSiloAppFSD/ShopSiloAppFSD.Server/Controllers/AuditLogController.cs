using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        // Log a user action
        [HttpPost("log")]
        public async Task<IActionResult> LogUserAction([FromQuery] int userId, [FromBody] string action)
        {
            if (string.IsNullOrEmpty(action))
            {
                return BadRequest("Action cannot be null or empty.");
            }

            try
            {
                await _auditLogRepository.LogUserActionAsync(action);
                return Ok();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Retrieve user activity logs
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserActivityLogs(int userId)
        {
            try
            {
                var logs = await _auditLogRepository.GetUserActivityLogsAsync(userId);
                return Ok(logs);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

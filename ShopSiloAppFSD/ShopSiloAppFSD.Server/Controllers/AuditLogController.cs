using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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

        // Export audit logs to Excel
        [HttpGet("export/{userId}")]
        public async Task<IActionResult> ExportUserActivityLogsToExcel(int userId)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var logs = await _auditLogRepository.GetUserActivityLogsAsync(userId);

                if (logs == null || !logs.Any())
                {
                    return NotFound("No logs found for the specified user.");
                }

                // Generate Excel file
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AuditLogs");
                    worksheet.Cells["A1"].Value = "User ID";
                    worksheet.Cells["B1"].Value = "Action";
                    worksheet.Cells["C1"].Value = "Timestamp";

                    var rowIndex = 2;
                    foreach (var log in logs)
                    {
                        worksheet.Cells[rowIndex, 1].Value = log.UserId;  // Replace with username if necessary
                        worksheet.Cells[rowIndex, 2].Value = log.Action;
                        worksheet.Cells[rowIndex, 3].Value = log.Timestamp.ToString("g");
                        rowIndex++;
                    }

                    // Convert to byte array
                    var fileBytes = package.GetAsByteArray();

                    // Return file
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AuditLogs.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting logs: {ex.Message}");
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.PFOM.DTOs;
using SafeCity.PFOM.Services;

namespace SafeCity.PFOM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Police,Emergency_Dispatcher,Fire_Fighter, City_Administrator")]
    public class FieldReportController : ControllerBase
    {
        private readonly IFieldReportService _fieldReportService;

        public FieldReportController(IFieldReportService fieldReportService)
        {
            _fieldReportService = fieldReportService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitFieldReport([FromBody] FieldReportRequest request)
        {
            try
            {
                // Token se Logged-in Officer ki ID nikalna
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User ID claim not found in token" });
                }
                // current logged in user he dispatcher hoga
                int officerId = int.Parse(userIdClaim.Value);

                // Service ko call karna 
                var response = await _fieldReportService.FieldReport(request, officerId);

                return Ok(new
                {
                    message = "Field Report successfully submitted!",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                // Basic Validations
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Business Logic errors Patrol not started, Wrong Officer ke liye
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [Authorize(Roles = "Emergency_Dispatcher,City_Administrator")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllFieldReport()
        {
            try
            {
                var response = await _fieldReportService.GetAllFeildReport();
                if (response == null)
                {
                    return NotFound("No Field Report Found");
                }
                else
                {
                    return Ok(new { message = "All Field Report List", data = response });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Emergency_Dispatcher,City_Administrator")]
        [HttpPatch("{ReportId:int}/review")]
        public async Task<IActionResult> ReviewFieldReport(int ReportId, [FromQuery] int newStatus)
        {
            try
            {
                var result = await _fieldReportService.ReviewFieldReportService(ReportId, newStatus);

                if (result == "Success")
                {
                    return Ok(new { message = $"Field Report {ReportId} ka status update ho gaya hai." });
                }

                return BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error: " + ex.Message });
            }
        }


        [Authorize(Roles = "Police,Emergency_Dispatcher,Fire_Fighter, City_Administrator")]
        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory()
        {
            try
            {
                // Token se UserId nikalna
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "UserId claim nahi mila." });

                int officerId = int.Parse(userIdClaim);

                // Service call
                var history = await _fieldReportService.GetMyReportHistoryService(officerId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
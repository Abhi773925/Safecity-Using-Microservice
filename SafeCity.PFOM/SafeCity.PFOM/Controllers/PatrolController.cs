using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.PFOM.DTOs;
using SafeCity.PFOM.Repositories;
using SafeCity.PFOM.Services;

namespace SafeCity.PFOM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatrolController : ControllerBase
    {
        private readonly IPatrolService _service;
        private readonly IPatrolRepository _patrolRepository;

        public PatrolController(IPatrolService service, IPatrolRepository patrolRepository)
        {
            _service = service;
            _patrolRepository = patrolRepository;
        }

        [Authorize(Roles = "Emergency_Dispatcher,City_Administrator,Compliance_Officer,Fire_Fighter")]
        [HttpPost("schedule")]
        public async Task<IActionResult> PatrolSchedule(PatrolScheduleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _service.PatrolSchedule(request);
                return Ok(new { message = "Patrol Scheduled Successfully", data = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Police,Emergency_Dispatcher,Fire_Fighter")]
        [HttpGet("my-patrols")]
        public async Task<IActionResult> MyPatrols()
        {
            try
            {

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User ID claim not found in token" });
                }
                // current logged in user he dispatcher hoga
                int userId = int.Parse(userIdClaim.Value);

                var response = await _service.MyPatrols(userId);
                if (response == null)
                {
                    return BadRequest("No Patrol scheduled for you");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize(Roles = "Police,Emergency_Dispatcher,Fire_Fighter")]
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdatePatrolStatusDto dto)
        {
            // Token se logged-in User ID nikalna
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim not found in token" });
            }
            // current logged in user he dispatcher hoga
            int userId = int.Parse(userIdClaim.Value);
            var result = await _patrolRepository.UpdatePatrolStatusAsync(dto.PatrolId, userId, dto.NewStatus);

            if (result == "Success")
                return Ok(new { message = $"Patrol status updated to {dto.NewStatus} successfully." });

            return BadRequest(new { message = result });
        }


        [Authorize(Roles = "Police,Emergency_Dispatcher,Fire_Fighter")]
        [HttpGet("my-patrol-history")]
        public async Task<IActionResult> GetMyPatrolHistory()
        {
            try
            {
                // Token se OfficerId nikalna
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "UserId claim missing in token." });

                int officerId = int.Parse(userIdClaim);

                // Service call
                var history = await _service.GetMyPatrolHistoryService(officerId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

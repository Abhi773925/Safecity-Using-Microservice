using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.DCR.DTOs;
using SafeCity.DCR.Services;

[Authorize(Roles = "City_Administrator,Emergency_Dispatcher,Police,Fire_Fighter")]
[Route("api/[controller]")]
[ApiController]
public class ResponseController : ControllerBase
{
    private readonly IResponseService _service;
    public ResponseController(IResponseService service) => _service = service;

    // deploy kro ek team ko us crisis ke corresponding
    [HttpPost("deploy")]
    public async Task<IActionResult> Deploy([FromBody] DeploymentRequest request)
    {
        try
        {
            var result = await _service.DeployAsync(request);
            return Ok(new { Message = "MISSION STARTED: Team Dispatched!", Data = result });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
    // us ciris ke liye dusra team reassign kr do and phla wala ko free kar do
    [HttpPatch("{id}/reassign/{newTeamId}")]
    public async Task<IActionResult> Reassign(int id, int newTeamId)
    {
        var result = await _service.ReassignAsync(id, newTeamId);
        if (!result) return BadRequest("Reassignment failed. Either team is busy or record not found.");
        return Ok("Team swapped successfully!");
    }

    // jo active team deployed hai uska details and corresponding the crisis details
    [HttpGet("active")]
    public async Task<IActionResult> GetActive() => Ok(await _service.GetAllActive());

    // deployment cancel krke team ko free kar dena
    [HttpDelete("cancel/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _service.CancelAsync(id);
        return result ? Ok("Deployment Cancelled. Team is now Available.") : NotFound();
    }

    // deployement jo kiye hai team wo team apna crisis ka response update krna
    [Authorize(Roles = "City_Administrator,Emergency_Dispatcher,Police,Fire_Fighter")]
    [HttpPatch("{id}/progress")]
    public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateProgressRequest request)
    {
        try
        {
            // JWT Token se TeamLeadId  nikalna
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("User ID not found in token.");

            int teamLeadId = int.Parse(userIdClaim);

            await _service.UpdateResponseStatus(id, teamLeadId, request);

            return Ok(new
            {
                Message = "Ground progress updated successfully!",
                CurrentStatus = request.NewStatus.ToString(),
                UpdateTime = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // closing the mission and free all the resource related to it
    [Authorize(Roles = "City_Administrator,Emergency_Dispatcher")]
    [HttpPost("{id}/close")]
    public async Task<IActionResult> CloseMission(int id, [FromBody] CloseMissionRequest request)
    {
        try
        {
            await _service.CloseMission(id, request);
            return Ok(new
            {
                Message = "Mission Resolved and Team is now Available for next task!",
                Status = "Closed"
            });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
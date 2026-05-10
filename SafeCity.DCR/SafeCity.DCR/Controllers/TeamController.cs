using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.DCR.DTOs;
using SafeCity.DCR.Services;

namespace SafeCity.DCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // Team Creation only City Administrator can create the Team

        [Authorize(Roles = "City_Administrator")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam(TeamRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _teamService.CreateTeam(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Get all the team details

        [Authorize(Roles = "City_Administrator")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllTeam()
        {
            try
            {
                var response = await _teamService.GetAllTeam();
                if (response == null)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Get all the team details who is active

        [Authorize(Roles = "City_Administrator")]
        [HttpGet("available")]
        public async Task<IActionResult> GetActiveTeamDetails()
        {
            try
            {
                var response = await _teamService.GetActiveTeamDetails();
                if (response == null)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // update the team status using teamid
        [Authorize(Roles = "City_Administrator")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTeamStatus([FromBody] UpdateTeamStatusRequest request, int id)
        {
            try
            {
                // Service call with await to update the team status
                await _teamService.UpdateTeamStatus(request.NewStatus, id);
                return Ok("Team Status Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

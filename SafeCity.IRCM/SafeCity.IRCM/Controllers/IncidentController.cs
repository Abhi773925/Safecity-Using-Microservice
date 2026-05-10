using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.HttpClients;
using SafeCity.IRCM.Services;

namespace SafeCity.IRCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _service;
        private readonly IIdentityService _identityService;
        private readonly IIncidentRetrivalService _incidentRetrivalService;

        public IncidentController(IIncidentService service, IIdentityService identityService, IIncidentRetrivalService incidentRetrivalService)
        {
            _service = service;
            _identityService = identityService;
            _incidentRetrivalService = incidentRetrivalService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> IncidentCreate(IncidentCreateRequest request)
        {
            try
            {
                // Authorization Header se Token nikalna
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized("Token missing in header");
                }

                // Bearer word hata kar sirf token lena
                string token = authHeader.Replace("Bearer ", "");

                // IdentityService se User details (CitizenID) nikalna
                var userDetails = await _identityService.GetLoggedInUsers(token);

                // CitizenID ko automatically fill karna
                request.CitizenID = userDetails.UserID;

                // validate Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _service.IncidentCreate(request);
                return Ok(new { message = "Incident Created Successfully", data = response });
            }
            catch (Exception ex)
            {
                // InnerException null ho sakta hai, isliye safe check
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(errorMsg);
            }
        }

        [HttpPatch("{incidentId:int}/status")]
        public async Task<IActionResult> UpdateIncidentStatus(int incidentId, [FromQuery] int option)
        {
            try
            {
                await _service.IncidentStatusUpdate(incidentId, option);
                return Ok(new { message = "Incident Status Updated Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // only police officer or the Dispatcher will be able to see the incident with the pending state.

        [Authorize(Roles = "Police, Emergency_Dispatcher, City_Administrator, Fire_Fighter")]
        [HttpGet("list")]

        public async Task<IActionResult> IncidentRetrival()
        {
            try
            {
                var response = await _incidentRetrivalService.IncidentRetrival();
                if (response == null)
                {
                    return NotFound("No Incident with Pending State Found");
                }
                return Ok(new { message = "Incident with pending state need to be solved", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Police, Emergency_Dispatcher, City_Administrator, Fire_Fighter")]
        [HttpGet("list/all")]
        public async Task<IActionResult> IncidentRetrivalAll()
        {
            try
            {
                var response = await _incidentRetrivalService.IncidentRetrivalAll();
                if (response == null)
                {
                    return NotFound("No Incident Found");
                }
                return Ok(new { message = "Incident Reported till now", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Citizen")]
        [HttpGet("my-incidents")]
        public async Task<IActionResult> IncidentRetrivalMine()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized("Token missing in header");
                }

                string token = authHeader.Replace("Bearer ", "");
                var userDetails = await _identityService.GetLoggedInUsers(token);
                var response = await _incidentRetrivalService.IncidentRetrivalByCitizenId(userDetails.UserID);

                if (response == null)
                {
                    return NotFound("No Incident Found For This Citizen");
                }

                return Ok(new { message = "Your incident history", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
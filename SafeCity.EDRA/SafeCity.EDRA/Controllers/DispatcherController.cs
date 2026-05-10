using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity_EDRADB.Enums;
using SafeCity.EDRA.DTOs;
using SafeCity.EDRA.HttpClients;
using SafeCity.EDRA.Services;
namespace SafeCity.EDRA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispatcherController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IResourceService _resourceService;
        public DispatcherController(IIncidentService incidentService, IResourceService resourceService)
        {
            _incidentService = incidentService;
            _resourceService = resourceService;
        }

        [Authorize(Roles = "Emergency_Dispatcher, City_Administrator")]
        [HttpGet("list")]
        public async Task<IActionResult> GetDispatches()
        {
            try
            {
                var dispatches = await _resourceService.GetAllDispatches();
                return Ok(new { message = "Dispatches Retrieved", data = dispatches });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> DispatcherDashboard()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader)) return Unauthorized("Token is missing");

            string token = authHeader.Replace("Bearer ", "");

            // getting the pending incident and available resource
            var incidents = await _incidentService.GetIncidentsAsync(token);
            var incidentPending = incidents
    .Where(temp => temp.Status == "Pending" || temp.Status == "InProgress")
    .ToList();
            var resources = await _resourceService.GetAllResource();
            return Ok(new { incidentPending, resources });
        }


        // asssigning the resource by the dispatcher
        [Authorize(Roles = "Emergency_Dispatcher")]
        [HttpPost("assign-resource")]
        public async Task<IActionResult> AssignResource(DispatchRequest request)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim not found in token" });
            }
            // current logged in user he dispatcher hoga
            int dispatcherId = int.Parse(userIdClaim.Value);
            request.DispatcherID = dispatcherId;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _resourceService.AssignResource(request);
                return Ok(new { message = "Resource has been Assigned to the Incident", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Emergency_Dispatcher")]
        [HttpPost("dispatch")]
        public async Task<IActionResult> DispatchResource([FromBody] DispatchRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null)
                    return Unauthorized("User ID not found");

                request.DispatcherID = int.Parse(userIdClaim.Value);
                var response = await _resourceService.AssignResource(request);
                return Ok(new { message = "Resource dispatched successfully", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //update travel status

        [Authorize(Roles = "Emergency_Dispatcher, Police, Fire_Fighter")]
        [HttpPatch("update-status/{dispatchId}")]
        public async Task<IActionResult> UpdateStatus(int dispatchId, [FromBody] DispatchStatusUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _resourceService.UpdateDispatchStatusAsync(dispatchId, request.Status);

                if (result)
                {
                    return Ok(new { message = $"Dispatch {dispatchId} status updated to {request.Status} successfully" });
                }
                return BadRequest("Failed to update status");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("complete-dispatch/{dispatchId}")]
        [Authorize(Roles = "Emergency_Dispatcher")]
        public async Task<IActionResult> CompleteDispatch(int dispatchId)
        {
            try
            {
                var result = await _resourceService.CompleteDispatchAsync(dispatchId);
                if (result)
                {
                    return Ok(new { message = "Dispatch resolved and Resource is now Available again." });
                }
                return BadRequest("Failed to complete dispatch.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
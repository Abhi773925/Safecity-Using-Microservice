using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.HttpClients;
using SafeCity.IRCM.Services;

namespace SafeCity.IRCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        //dependency Injection goes here
        private readonly IIdentityService _identityService;
        private readonly ICaseCreateService _caseCreateService;
        private readonly ICaseClosingService _caseClosingService;
        public CaseController(IIdentityService identityService, ICaseCreateService caseCreateService, ICaseClosingService caseClosingService)
        {
            _identityService = identityService;
            _caseCreateService = caseCreateService;
            _caseClosingService = caseClosingService;
        }

        //case creation controller

        [Authorize(Roles = "Police, Emergency_Dispatcher, City_Administrator")]
        [HttpPost("create/{IncidentID:int}")]

        public async Task<IActionResult> CaseCreate(CaseCreateRequest request, int IncidentID)
        {
            try
            {
                // validate the Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                request.IncidentID = IncidentID;

                // to find the citizen id based on the user token who logged in

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
                request.AssignedOfficerID = userDetails.UserID;

                var response = await _caseCreateService.CaseCreate(request);
                return Ok(new { message = $"Case Created Successfully for the ${IncidentID}", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //case closing controller i.e solving a case and closing an incident

        [Authorize(Roles = "Police, Emergency_Dispatcher, City_Administrator, Fire_Fighter")]
        [HttpPost("submission/{CaseId:int}")]

        public async Task<IActionResult> CaseClosing(CaseClosingRequest request, int CaseId)
        {
            try
            {
                // validating the Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _caseClosingService.CaseClosing(request, CaseId);
                return Ok(new { message = "Case Closed Successfully and Marked the Incident as Solved" });
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        //Get All Case

        [Authorize(Roles = "Police, Emergency_Dispatcher, City_Administrator, Fire_Fighter")]
        [HttpGet("all-cases")]
        public async Task<IActionResult> GetAllCase()
        {
            try
            {
                var response = await _caseCreateService.GetAllCase();
                return Ok(new { message = "All Case List", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Citizen")]
        [HttpGet("my-cases")]
        public async Task<IActionResult> GetMyCase()
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
                var response = await _caseCreateService.GetCaseByCitizenId(userDetails.UserID);

                return Ok(new { message = "Your case history", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}

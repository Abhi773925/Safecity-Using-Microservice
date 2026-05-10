using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.DCR.DTOs;
using SafeCity.DCR.Services;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CrisisController : ControllerBase
    {
        private readonly ICrisisService _crisisService;
        public CrisisController(ICrisisService crisisService)
        {
            _crisisService = crisisService;
        }

        // Sirf Admin ya Dispatcher hi Crisis declare kar sakte hain
        [Authorize(Roles = "City_Administrator,Emergency_Dispatcher")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CrisisRequest request)
        {
            try
            {
                var result = await _crisisService.CreateCrisis(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Crisis By the Id
        [Authorize(Roles = "City_Administrator,Emergency_Dispatcher,Fire_Fighter")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCrisisByID(int id)
        {
            var response = await _crisisService.GetCrisisDetails(id);
            return Ok(response);
        }

        // Ye view Dispatchers aur Admins dono dekh sakte hain
        [Authorize(Roles = "City_Administrator,Emergency_Dispatcher,Fire_Fighter")]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _crisisService.GetCrises(true);
            return Ok(result);
        }

        // Audit view aksar sirf Admin ke liye hoti hai
        [Authorize(Roles = "City_Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _crisisService.GetCrises(false);
            return Ok(result);
        }

        // Situation escalate karna Dispatcher ka kaam hai
        [Authorize(Roles = "City_Administrator,Emergency_Dispatcher")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromQuery] CrisisStatus? status, [FromQuery] CrisisSeverity? severity)
        {
            var result = await _crisisService.UpdateCrisisDetail(id, status, severity);
            if (!result) return NotFound("Crisis record not found.");
            return Ok("Crisis Updated Successfully");
        }
    }
}
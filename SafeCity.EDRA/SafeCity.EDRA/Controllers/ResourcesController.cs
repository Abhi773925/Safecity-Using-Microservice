using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.EDRA.DTOs;
using SafeCity.EDRA.Services;
namespace SafeCity.EDRA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;
        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [Authorize(Roles = "Emergency_Dispatcher, City_Administrator, Police, Fire_Fighter")]
        [HttpPost("add")]
        public async Task<IActionResult> AddResource(ResourceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await _resourceService.AddResource(request);
                return Ok(new { message = "Resource Added Successfully", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Emergency_Dispatcher, City_Administrator, Police, Fire_Fighter")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllResources()
        {
            try
            {
                var resources = await _resourceService.GetAllResources();
                return Ok(new { message = "Resources Retrieved", data = resources });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Emergency_Dispatcher, City_Administrator")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateResource(int id, ResourceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await _resourceService.UpdateResource(id, request);
                return Ok(new { message = "Resource Updated", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

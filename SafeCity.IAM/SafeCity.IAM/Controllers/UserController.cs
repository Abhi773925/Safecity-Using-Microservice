using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCity.IAM.DTOs;
using SafeCity.IAM.Services;

namespace SafeCity.IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest request)
        {
            // Model validation check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.RegisterUser(request);

                if (response == null)
                {
                    return BadRequest("Registration failed.");
                }

                return Ok(new { message = "User Registered Successfully", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginUser(LoginRequest loginRequest)
        {

            //validate the Model State
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.LoginUser(loginRequest);
                if (response == null)
                {
                    return BadRequest("Login failed.");
                }

                return Ok(new { message = "User Logged in Successfully", data = response });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //City Administrator can only delete the User
        [Authorize(Roles = "City_Administrator,Citizen")]
        [HttpDelete("delete/{UserId:int}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            try
            {
                await _userService.DeleteUser(UserId);
                return Ok(new { message = "User Deleted Successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //City Administrator can Update Anyone Details
        [Authorize(Roles = "City_Administrator")]
        [HttpPut("update/{UserId:int}")]
        public async Task<IActionResult> UpdateUser(UserUpdateRequest request, int UserId)
        {
            try
            {
                //Validating the Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _userService.UpdateUser(request, UserId);
                return Ok(new { message = "User Updated Successfully", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Any Authorised User Can Change their Password

        [Authorize]
        [HttpPut("change/password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                //Checking the Model State
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _userService.ChangePassword(request);
                return Ok(new { message = "User Password Change Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Admin Can Only Reset Anyone Password with their Email Id We can Modify Also to each Invidual Users Later on
        [Authorize(Roles = "City_Administrator")]
        [HttpPut("forgot/password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _userService.ForgotPassword(request);

                return Ok(new { message = "User Password Reset Successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("internal/{userId:int}")]
        public async Task<IActionResult> InternalGetUserById(int userId)
        {
            try
            {
                var response = await _userService.GetUserById(userId);
                if (response == null)
                {
                    return NotFound("User Not Found");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

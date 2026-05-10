using Microsoft.IdentityModel.Tokens;
using SafeCity.IAM.DTOs;
using SafeCity.IAM.Repositories;
using SafeCity.IAM.Utility;
using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SafeCity.IAM.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginUser(LoginRequest request)
        {
            if (request == null) throw new Exception(ValidationHelper.RequestNull.ToString());


            var user = await _userRepository.LoginUser(request);

            if (user == null) throw new Exception(string.Join("| ", "User Not Found"));

            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            return LoginResponseExtension.ToUserLoginResponse(accessToken, refreshToken);
        }

        public async Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(ValidationHelper.RequestNull);
            }
            var errorList = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errorList.Add("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errorList.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errorList.Add("Password is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                errorList.Add("Phone number is required.");

            if (request.RoleID <= 0)
                errorList.Add("A valid Role ID must be provided.");

            if (errorList.Count > 0)
            {
                throw new Exception(string.Join(" |", errorList));
            }

            // extra validation check
            var emailResult = ValidationHelper.EmailHelper(request.Email);
            if (emailResult != null) errorList.Add(emailResult);

            var passwordResult = ValidationHelper.PasswordHelper(request.Password);
            if (passwordResult != null) errorList.Add(passwordResult);

            var phoneResult = ValidationHelper.PhoneHelper(request.Phone);
            if (phoneResult != null) errorList.Add(phoneResult);

            if (errorList.Count > 0)
            {
                throw new FormatException(string.Join(" |", errorList));
            }

            //if above all validation pass then invoke the repository layer

            //hash the password
            var response = await _userRepository.RegisterUser(request);
            return response;

        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,((UserRoleOption)user.RoleID).ToString()),
                new Claim("UserId", user.UserID.ToString())


            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var securityAlgorithms = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDetails = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                        signingCredentials: securityAlgorithms
                        );

            return new JwtSecurityTokenHandler().WriteToken(tokenDetails);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public Task DeleteUser(int UserId)
        {
            var response = _userRepository.DeleteUser(UserId);
            return response;
        }

        public async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request, int UserId)
        {
            //check if the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //check if the user id is negative
            if (UserId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(UserId));
            }
            //check for the validation while updation
            var errorList = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errorList.Add("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errorList.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errorList.Add("Password is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                errorList.Add("Phone number is required.");

            if (request.RoleID <= 0)
                errorList.Add("A valid Role ID must be provided.");

            if (errorList.Count > 0)
            {
                throw new Exception(string.Join(" |", errorList));
            }

            // extra validation check
            var emailResult = ValidationHelper.EmailHelper(request.Email);
            if (emailResult != null) errorList.Add(emailResult);

            var passwordResult = ValidationHelper.PasswordHelper(request.Password);
            if (passwordResult != null) errorList.Add(passwordResult);

            var phoneResult = ValidationHelper.PhoneHelper(request.Phone);
            if (phoneResult != null) errorList.Add(phoneResult);

            if (errorList.Count > 0)
            {
                throw new FormatException(string.Join(" |", errorList));
            }
            else
            {
                var response = await _userRepository.UpdateUser(request, UserId);
                return response;
            }
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            //check the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                //verify the new password logic
                var passwordResult = ValidationHelper.PasswordHelper(request.NewPassword);
                if (passwordResult != null)
                {
                    throw new Exception(passwordResult);
                }
                //Invoking the next repository layer
                await _userRepository.ChangePassword(request);

            }

        }

        public async Task ForgotPassword(ForgotPasswordRequest request)
        {
            //check if the request is null
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                var emailError = ValidationHelper.EmailHelper(request.Email);
                if (emailError != null)
                {
                    throw new Exception(emailError);
                }
                else
                {
                    if (request.NewPassword == null || request.ConfirmNewPassword == null)
                    {
                        throw new Exception(nameof(request));
                    }
                    await _userRepository.ForgotPassword(request);
                }
            }
        }

        public async Task<object?> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return null;
            }

            return new
            {
                UserID = user.UserID,
                Name = user.Name,
                RoleID = user.RoleID,
                RoleName = ((UserRoleOption)user.RoleID).ToString(),
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status.ToString()
            };
        }
    }
}

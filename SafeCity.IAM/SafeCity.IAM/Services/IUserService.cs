using SafeCity.IAM.DTOs;

namespace SafeCity.IAM.Services
{
    public interface IUserService
    {
        public Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request);

        public Task<LoginResponse> LoginUser(LoginRequest request);

        public Task DeleteUser(int UserId);

        public Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request, int UserId);

        public Task ChangePassword(ChangePasswordRequest request);
        public Task ForgotPassword(ForgotPasswordRequest request);
        public Task<object?> GetUserById(int userId);

    }
}

using SafeCity.IAM.DTOs;
using SafeCity.IAMDB.Entities;

namespace SafeCity.IAM.Repositories
{
    public interface IUserRepository
    {

        public Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request);
        public Task<User> LoginUser(LoginRequest request);
        public Task DeleteUser(int UserId);
        public Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request, int UserId);
        public Task ChangePassword(ChangePasswordRequest request);
        public Task ForgotPassword(ForgotPasswordRequest request);
        public Task<User?> GetUserById(int userId);

    }
}

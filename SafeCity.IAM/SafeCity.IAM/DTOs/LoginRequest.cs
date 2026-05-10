using SafeCity.IAMDB.Entities;

namespace SafeCity.IAM.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public User ToUserLoginRequest()
        {
            return new User { Email = Email, Password = Password };
        }
    }
}

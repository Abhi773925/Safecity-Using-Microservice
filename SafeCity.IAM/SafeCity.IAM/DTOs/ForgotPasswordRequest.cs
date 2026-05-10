using SafeCity.IAMDB.Entities;

namespace SafeCity.IAM.DTOs
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmNewPassword { get; set; } = default!;

        public User ToForgotPasswordRequest()
        {
            return new User()
            {
                Email = Email,
                Password = NewPassword,
            };
        }
    }
}

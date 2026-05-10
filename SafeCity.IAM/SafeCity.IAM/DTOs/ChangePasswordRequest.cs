using SafeCity.IAMDB.Entities;

namespace SafeCity.IAM.DTOs
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; } = default!;
        public string ExistingPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;

        public User ToChangePasswordRequest()
        {
            return new User
            {
                Email = Email,
                Password = NewPassword,
            };
        }
    }
}

using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;

namespace SafeCity.IAM.DTOs
{
    public class UserRegisterRequest
    {
        public string Name { get; set; } = default!;
        public int RoleID { get; set; }
        public string Email { get; set; } = default!;
        public string Phone { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string Password { get; set; } = default!;


        public User ToUserRegisterRequest()
        {
            return new User
            {
                Name = Name,
                RoleID = RoleID,
                Email = Email,
                Phone = Phone,
                Password = Password,
            };
        }

    }
}

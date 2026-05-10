using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;

namespace SafeCity.IAM.DTOs
{
    public class UserUpdateRequest
    {
        public string Name { get; set; } = default!;
        public int RoleID { get; set; }
        public string Email { get; set; } = default!;
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string Password { get; set; }
        public User ToUserUpdateRequest()
        {
            return new User
            {
                Name = Name,
                RoleID = RoleID,
                Email = Email,
                Phone = Phone,
                Status = Status,
                Password = Password,
            };
        }
    }
}

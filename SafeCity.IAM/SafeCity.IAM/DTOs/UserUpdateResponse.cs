using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;

namespace SafeCity.IAM.DTOs
{
    public class UserUpdateResponse
    {
        public string Name { get; set; } = default!;
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; } = default!;
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string StatusName { get; set; }
        public string Password { get; set; }
    }
    public static class UserUpdateResponseExtenstion
    {
        public static UserUpdateResponse ToUserUpdateResponse(User user)
        {
            return new UserUpdateResponse
            {
                Name = user.Name,
                RoleID = user.RoleID,
                RoleName = ((UserRoleOption)user.RoleID).ToString(),
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status,
                StatusName = user.Status.ToString(),
                Password = user.Password

            };
        }
    }
}

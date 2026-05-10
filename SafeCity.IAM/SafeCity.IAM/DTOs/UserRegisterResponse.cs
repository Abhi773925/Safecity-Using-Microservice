using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;

namespace SafeCity.IAM.DTOs
{
    public class UserRegisterResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = default!;
        public int RoleID { get; set; }
        public string Email { get; set; } = default!;
        public string Phone { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
    }
    public static class UserRegisterResponseExtension
    {
        public static UserRegisterResponse ToUserRegisterResponse(User user)
        {
            return new UserRegisterResponse
            {
                UserId = user.UserID,
                Name = user.Name,
                RoleID = user.RoleID,
                Email = user.Email,
                Phone = user.Phone,
                Status = (user.Status).ToString(),
                RoleName = ((UserRoleOption)user.RoleID).ToString(),
            };
        }
    }
}

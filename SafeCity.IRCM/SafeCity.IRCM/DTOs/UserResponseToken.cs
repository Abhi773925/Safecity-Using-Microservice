namespace SafeCity.IRCM.DTOs
{
    public class UserResponseToken
    {
        public string Token { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public int UserID { get; set; }
    }
}
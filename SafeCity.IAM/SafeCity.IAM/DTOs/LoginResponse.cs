namespace SafeCity.IAM.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public static class LoginResponseExtension
    {
        public static LoginResponse ToUserLoginResponse(string accessToken, string refreshToken)
        {
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}

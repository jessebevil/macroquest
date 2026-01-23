namespace RGAuth.Models
{
    using Newtonsoft.Json;

    public class TokenStorage
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
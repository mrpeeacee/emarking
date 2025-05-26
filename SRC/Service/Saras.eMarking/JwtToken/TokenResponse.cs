namespace TokenLibrary
{
    public class TokenResponse
    { 
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public double RefreshInterval { get; set; }
        public string? RefKey { get; set; }
    }
}
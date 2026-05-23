namespace Student_Management.DTO.AccountDto
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}

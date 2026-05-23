using Student_Management.DTO.AccountDto;
using Student_Management.Models;

namespace Student_Management.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDto> IssueTokensAsync(User user);
        Task<TokenResponseDto?> RefreshAsync(string refreshToken);
    }
}

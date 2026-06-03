using KMSTraining.API.Application.DTOs;

namespace KMSTraining.API.Application.Services;

public interface ITokenService
{
    string GenerateToken(int userId, string username, string email);
}

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}

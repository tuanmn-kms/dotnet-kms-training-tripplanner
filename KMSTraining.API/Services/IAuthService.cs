using KMSTraining.API.DTOs;
using KMSTraining.API.Models;

namespace KMSTraining.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}

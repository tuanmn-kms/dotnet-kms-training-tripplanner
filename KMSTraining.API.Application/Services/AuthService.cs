using BCrypt.Net;
using KMSTraining.API.Application.DTOs;
using KMSTraining.API.Application.Mapping;
using KMSTraining.API.Domain.Exceptions;
using KMSTraining.API.Domain.Interfaces;

namespace KMSTraining.API.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if username already exists
        if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
        {
            throw new DuplicateEntityException("Username already exists");
        }

        // Check if email already exists
        if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
        {
            throw new DuplicateEntityException("Email already exists");
        }

        // Hash password
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create user
        var user = _mapper.MapRegisterDtoToUser(passwordHash, registerDto);

        // Add to repository
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate token
        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by username or email
        var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.UsernameOrEmail)
            ?? await _unitOfWork.Users.GetByEmailAsync(loginDto.UsernameOrEmail);

        // Verify user exists and password is correct
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new AuthenticationException("Invalid credentials");
        }

        // Generate token
        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}

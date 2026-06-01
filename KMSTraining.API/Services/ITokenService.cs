namespace KMSTraining.API.Services;

public interface ITokenService
{
    string GenerateToken(int userId, string username, string email);
}

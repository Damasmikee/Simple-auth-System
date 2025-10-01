using AuthOnion.Api.Domain.Entities;

namespace AuthOnion.Api.Infrastructure.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        DateTime GetExpiry();
    }
}

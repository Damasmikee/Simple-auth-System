using AuthOnion.Api.Application.DTOs;
using AuthOnion.Api.Application.Interfaces;
using AuthOnion.Api.Domain.Entities;
using AuthOnion.Api.Domain.Interfaces;
using AuthOnion.Api.Infrastructure.Services;

namespace AuthOnion.Api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwt;

        public AuthService(IUserRepository repo, IPasswordHasher hasher, IJwtService jwt)
        {
            _repo = repo;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existing = await _repo.GetByEmailAsync(request.Email!);
            if (existing != null)
                throw new Exception("User with this email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = _hasher.Hash(request.Password!)
            };

            await _repo.AddAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _repo.GetByEmailAsync(request.Email!);
            if (user == null) throw new Exception("Invalid credentials");

            var valid = _hasher.Verify(request.Password!, user.PasswordHash!);
            if (!valid) throw new Exception("Invalid credentials");

            var token = _jwt.GenerateToken(user);
            return new AuthResponse { Token = token, ExpiresAt = _jwt.GetExpiry() };
        }
    }
}

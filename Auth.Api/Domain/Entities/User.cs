using System;

namespace AuthOnion.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}

using api.Domain.Models;
using api.Infrastructure.Database.Entities;

namespace api.Domain.Repositories
{
    public class FindRequest
    {
        public string? Email { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountDigit { get; set; }
    }


    public interface IUserRepository
    {
        Task<User> Create(UserModel user);
        Task<User?> GetByEmail(string email);
        Task<User> GetById(Guid id);
        Task<User?> Find(FindRequest request);        
    }
};



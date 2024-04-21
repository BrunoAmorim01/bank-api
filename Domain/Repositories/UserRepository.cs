using api.Domain.Models;
using api.Infrastructure.Database.Entities;

namespace api.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(UserModel user);
        Task<User?> GetByEmail(string email);
        Task<User> GetById(Guid id);
    }
};



using DSAR.Models;
using Microsoft.AspNetCore.Identity;

namespace DSAR.Repositories
{
    public interface IUserRepository
    {
       IEnumerable<User> GetAll();
        User GetById(string Id);
        void Create(User user);
        void UpdateTerms(User user);
        void Update(User user);
        void Delete(string Id);
        void Save();
    }
}

using DSAR.Models;

namespace DSAR.Interfaces
{
    public interface ICityRepository
    {
        IEnumerable<City> GetAll();
        City GetById(string Id);
        void Create(City user);
        void Update(City user);
        void Delete(string Id);
        void Save();
    }
}

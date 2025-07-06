using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DSAR.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;
        

        public CityRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Create(City user)
        {
            throw new NotImplementedException();
        }

        public void Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<City> GetAll()
        {
            var allCity = _context.City;
            return allCity;
        }

        public City GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(City user)
        {
            throw new NotImplementedException();
        }
    }
}

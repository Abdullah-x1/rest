using Microsoft.AspNetCore.Mvc.Rendering;

namespace DSAR.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Sector> Sectors { get; set; }
    }
}

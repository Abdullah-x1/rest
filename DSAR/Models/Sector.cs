using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class Sector
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; } // Navigation property
        public ICollection<User> Users { get; set; }
        public ICollection<Department> Departments { get; set; }
    }
}

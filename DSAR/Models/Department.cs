using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        [ForeignKey("Sector")]
        public int SectorId { get; set; }

        public Sector Sector { get; set; } // Navigation property
        // Navigation: One-to-Many
        public ICollection<User> Users { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}

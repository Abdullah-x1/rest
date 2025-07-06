using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class Section
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } // Navigation property
        public ICollection<User> Users { get; set; }

    }
}

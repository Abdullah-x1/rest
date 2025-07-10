using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class User : IdentityUser
    {

        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public string UserId { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        [ForeignKey("Section")]
        public int SectionId { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }

        public bool Active { get; set; } = true;

        [ForeignKey("Sector")]
        public int? SectorId { get; set; }

        public Department Department { get; set; } // Navigation property
        public Section Section { get; set; } // Navigation property
        public City City { get; set; } // Navigation property
        public Sector Sector { get; set; } // Navigation property
        public ICollection<FormData> Forms { get; set; }
        public ICollection<History> Histories { get; set; }

        public ICollection<CaseStudy> CaseStudies { get; set; }
    }

}

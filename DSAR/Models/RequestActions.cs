using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class RequestActions
    {
        [Key]
        public int ActionId { get; set; }
        [ForeignKey("Request")]
        public int RequestId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        [ForeignKey("Section")]
        public int SectionId { get; set; }
        [ForeignKey("Levels")]
        public int LevelId { get; set; }

        public FormData FormData { get; set; } // Navigation property
        public User User { get; set; } // Navigation property
        public Status Status { get; set; } // Navigation property
        public Levels Levels { get; set; } // Navigation property
        public Department Department { get; set; } // Navigation property
        public Section Section { get; set; } // Navigation property
    }
}

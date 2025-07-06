using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class CaseStudy
    {
        [Key]
        public int CaseId { get; set; }
        [ForeignKey("RequestId")]
        public int RequestId { get; set; }
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public string? WorkTeam { get; set; }
        public string? Notes { get; set; }
        public string? CreatedAt { get; set; }
        public string? restriction { get; set; }
        public FormData Request { get; set; }
        public User User { get; set; }

        public ICollection<CaseStudyAttachmentMetadata> Attachments { get; set; } = new List<CaseStudyAttachmentMetadata>();

    }
}

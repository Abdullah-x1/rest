using DSAR.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.ViewModels
{
    public class CaseStudyViewModel
    {
        public int CaseId { get; set; }
        public string Id { get; set; }
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WorkTeam { get; set; }
        public string Notes { get; set; }
        public string CreatedAt { get; set; }
        public string restriction { get; set; }

        public string? SectionNotes { get; set; }
        public string? DepartmentNotes { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();
    }
}

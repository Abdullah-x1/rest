using DSAR.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.ViewModels
{
    public class RequestViewModel
    {   
        public int RequestId { get; set; }
        public int ReuqestNumber { get; set; }
        public string UserId { get; set; }
        public Double RequestNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }


        //Request Actions
        public int ActionId { get; set; }
        public int StatusId { get; set; }
        public int LevelId { get; set; }
        public string StatusName { get; set; }
        //Case Study
        public int CaseId { get; set; }
        public string? WorkTeam { get; set; }
        public string? Notes { get; set; }
        public string restriction { get; set; }

        public string? Attachment { get; set; }
        public string? CreatedAt { get; set; }

        //form fields
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Depend { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? RepeatLimit { get; set; }
        public string? Fees { get; set; }
        public string? Cities { get; set; }
        public string? TargetAudience { get; set; }
        public string? DepName { get; set; }
        public string? ExpectedOutput1 { get; set; }
        public string? ExpectedOutput2 { get; set; }
        public string? ApprovedTemplate { get; set; }
        public string? DetailedInfo { get; set; }
        public string? RequiredConditions { get; set; }

        public string? Workflow { get; set; }
        public string? UploadsRequired { get; set; }
        public string? Documents { get; set; }

        public string? Timeline { get; set; }
        public string? SystemNeeded { get; set; }
        public string? Cities2 { get; set; } // to avoid collision with existing Cities
        public string? DepartmentHeadName { get; set; }
        public string? AdditionalNotes { get; set; } //
        public string? FilePath { get; set; } // Store file name or path

        [NotMapped]

        public IFormFile? Attachment1 { get; set; }

        [NotMapped]
        public IFormFile? Attachment2 { get; set; }

        [NotMapped]
        public IFormFile? Attachment3 { get; set; }

        [NotMapped]
        public IFormFile? WorkflowFile { get; set; }

        [NotMapped]
        public IFormFile? UploadsRequiredFile { get; set; }

        [NotMapped]
        public IFormFile? DocumentsFile { get; set; }

        //Descriptions
        public List<DescriptionEntry> Descriptions { get; set; } = new();
        public string ReturnUrl { get; set; } // Add this property
        public int FormId { get; set; }  // Add this property

        //notes
        public string? SectionNotes { get; set; }
        public string? DepartmentNotes { get; set; }




        public ICollection<AttachmentMetadata> Attachments { get; set; } = new List<AttachmentMetadata>();
        //public ICollection<DescriptionEntry> Descriptions { get; set; } = new List<DescriptionEntry>();
    }
}

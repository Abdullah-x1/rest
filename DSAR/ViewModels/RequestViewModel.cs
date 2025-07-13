using DSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "اسم الخدمة مطلوب")]

        public string Field1 { get; set; }

        [Required(ErrorMessage = "نوع الخدمة مطلوب")]

        public string Field2 { get; set; }
        [Required(ErrorMessage = "الوصف التفصيلي مطلوب")]

        public string? Field3 { get; set; }
        public string? Depend { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? RepeatLimit { get; set; }
        [Required(ErrorMessage = "الرسوم مطلوبه")]

        public string Fees { get; set; }
        public string? Cities { get; set; }
        [Required(ErrorMessage = "الفئة المستهدفة مطلوبه")]

        public string TargetAudience { get; set; }
        [Required(ErrorMessage = " اسم الإدارة مطلوب")]
        public string DepName { get; set; }
        public string? ExpectedOutput1 { get; set; }
        public string? ExpectedOutput2 { get; set; }
        public string? ApprovedTemplate { get; set; }
        public string? DetailedInfo { get; set; }
        public string? RequiredConditions { get; set; }

        [Required(ErrorMessage = " مسار العمل مطلوب")]
        public string Workflow { get; set; }
        public string? UploadsRequired { get; set; }
        public string? Documents { get; set; }

        [Required(ErrorMessage = " المدة الزمنية  مطلوبه")]
        public string Timeline { get; set; }
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

        public int Attachment1Id { get; set; }
        public string Attachment1Name { get; set; }

        public int Attachment2Id { get; set; }
        public string Attachment2Name { get; set; }

        public int Attachment3Id { get; set; }
        public string Attachment3Name { get; set; }

        public int WorkflowAttachmentId { get; set; }
        public string WorkflowName { get; set; }

        public int UploadsRequiredAttachmentId { get; set; }
        public string UploadsRequiredName { get; set; }

        public int DocumentsAttachmentId { get; set; }
        public string DocumentsName { get; set; }

        //Descriptions
        public List<DescriptionEntry> Descriptions { get; set; } = new();
        public string ReturnUrl { get; set; } // Add this property
        public int FormId { get; set; }  // Add this property

        //notes
        public string? SectionNotes { get; set; }
        public string? DepartmentNotes { get; set; }

        public IEnumerable<HistoryViewModel> History { get; set; }



        public ICollection<AttachmentMetadata> Attachments { get; set; } = new List<AttachmentMetadata>();
        //public ICollection<DescriptionEntry> Descriptions { get; set; } = new List<DescriptionEntry>();

        public int DepartmentId { get; set; }

        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();

    }
}

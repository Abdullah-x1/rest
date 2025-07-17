using DSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.ViewModels
{
    public class RequestViewModel
    {   
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public Double RequestNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }

        public bool TermsAccepted { get; set; }

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
        [StringLength(250)]
        public string? ServiceName { get; set; }              // يتم كتابة اسم الخدمة المطلوبة  

        [Required(ErrorMessage = "نوع الخدمة مطلوب")]
        [StringLength(250)]

        public string? ServiceTypeAndLocation { get; set; }   // يتم تحديد نوع الخدمة وموقعها  
        [Required(ErrorMessage = "الوصف التفصيلي مطلوب")]
        [StringLength(1000)]

        public string? ServiceDescription { get; set; }       // يتم كتابة وصف تفصيلي للخدمة المطلوب تطويرها  
        public string? HasDependency { get; set; }            // هل توجد اعتمادية على خدمات حالية؟  
        [StringLength(250)]

        public string? DependencyDetails { get; set; }        // توضيح الاعتمادية ان وجد  
        [StringLength(250)]

        public string? ProcedureNumber { get; set; }          // رقم الإجراء الإداري إن وجد  

        [StringLength(250)]
        public string? RepeatLimit { get; set; }

        [Required(ErrorMessage = "الرسوم مطلوبه")]
        [StringLength(250)]

        public string Fees { get; set; }
        public string? Cities { get; set; }
        [Required(ErrorMessage = "الفئة المستهدفة مطلوبه")]

        public string TargetAudience { get; set; }
        [Required(ErrorMessage = " اسم الإدارة مطلوب")]
        public string DepName { get; set; }

        [StringLength(1000)]
        public string? ExpectedOutput1 { get; set; }
        [StringLength(1000)]

        public string? ExpectedOutput2 { get; set; }
        [StringLength(1000)]

        public string? ApprovedTemplate { get; set; }
        [StringLength(1000)]

        public string? DetailedInfo { get; set; }
        [StringLength(1000)]

        public string? RequiredConditions { get; set; }

        [Required(ErrorMessage = " مسار العمل مطلوب")]
        [StringLength(250)]
        public string Workflow { get; set; }
        [StringLength(1000)]

        public string? UploadsRequired { get; set; }
        [StringLength(250)]

        public string? Documents { get; set; }

        [Required(ErrorMessage = " المدة الزمنية  مطلوبه")]
        [StringLength(250)]

        public string Timeline { get; set; }
        [StringLength(1000)]

        public string? SystemNeeded { get; set; }
        [StringLength(250)]

        public string? Cities2 { get; set; } // to avoid collision with existing Cities
        [StringLength(250)]

        public string? DepartmentHeadName { get; set; }
        [StringLength(250)]

        public string? AdditionalNotes { get; set; } //

        [NotMapped]
        public List<IFormFile> Attachment1 { get; set; }

        [NotMapped]
        public List<IFormFile> Attachment2 { get; set; }

        [NotMapped]
        public List<IFormFile> Attachment3 { get; set; }

        [NotMapped]
        public List<IFormFile> WorkflowFile { get; set; }

        [NotMapped]
        public List<IFormFile> UploadsRequiredFile { get; set; }

        [NotMapped]
        public List<IFormFile> DocumentsFile { get; set; }

        public List<int> Attachment1Id { get; set; } = new();
        public List<string> Attachment1Name { get; set; } = new();

        public List<int> Attachment2Id { get; set; }
        public List<string> Attachment2Name { get; set; }

        public List<int> Attachment3Id { get; set; }
        public List<string> Attachment3Name { get; set; }

        

        public List<int> WorkflowAttachmentId { get; set; }
        public List<string> WorkflowName { get; set; }

        public List<int> UploadsRequiredAttachmentId { get; set; }
        public List<string> UploadsRequiredName { get; set; }

        public List<int> DocumentsAttachmentId { get; set; }
        public List<string> DocumentsName { get; set; }

        //Descriptions
        public List<DescriptionEntry> Descriptions { get; set; } = new();
        public string ReturnUrl { get; set; } // Add this property
        public int FormId { get; set; }  // Add this property

        //notes
        public string? SectionNotes { get; set; }
        public string? DepartmentNotes { get; set; }
        public string? ITNotes { get; set; }
        public string? ApplicationNotes { get; set; }
        public IEnumerable<HistoryViewModel> History { get; set; }
        public List<int> CaseStudyAttachmentIds { get; set; } = new List<int>();
        public List<string> CaseStudyAttachmentNames { get; set; } = new List<string>();

        public ICollection<AttachmentMetadata> Attachments { get; set; } = new List<AttachmentMetadata>();
        //public ICollection<DescriptionEntry> Descriptions { get; set; } = new List<DescriptionEntry>();

        public int DepartmentId { get; set; }

        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<AuthorizedContactEntry> AuthorizedContacts { get; set; } = new();

        //public int CityId { get; set; }
        //public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();


    }
}

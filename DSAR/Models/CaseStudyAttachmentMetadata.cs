namespace DSAR.Models
{
    public class CaseStudyAttachmentMetadata
    {
        public int Id { get; set; }  // PK

        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public string FieldName { get; set; }

        // FK to CaseStudy
        public int CaseStudyId { get; set; }
        public CaseStudy CaseStudy { get; set; }

        // 1:1 navigation to AttachmentData, sharing PK
        public CaseStudyAttachmentData CaseStudyAttachmentData { get; set; }
    }
}

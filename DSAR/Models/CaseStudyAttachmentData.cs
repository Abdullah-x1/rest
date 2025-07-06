namespace DSAR.Models
{
    public class CaseStudyAttachmentData
    {
        public int Id { get; set; }  // PK and FK to CaseStudyAttachmentMetadata
        public byte[] Data { get; set; }

        public CaseStudyAttachmentMetadata CaseStudyAttachmentMetadata { get; set; }
    }
}

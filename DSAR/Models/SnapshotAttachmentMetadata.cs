
namespace DSAR.Models
{
    public class SnapshotAttachmentMetadata
    {
        public int Id { get; set; }
        public int SnapshotFormDataId { get; set; }
        public SnapshotFormData SnapshotFormData { get; set; }
        public SnapshotAttachmentData SnapshotAttachmentData { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public string FieldName { get; set; } // Add this

    }
}

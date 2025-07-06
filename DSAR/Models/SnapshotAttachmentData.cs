namespace DSAR.Models
{
    public class SnapshotAttachmentData
    {
        public int Id { get; set; }
        public int SnapshotAttachmentMetadataId { get; set; }
        public SnapshotAttachmentMetadata SnapshotAttachmentMetadata { get; set; }
        public byte[] Data { get; set; }
    }
}

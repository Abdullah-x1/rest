namespace DSAR.Models
{
    public class AttachmentData
    {


        public int Id { get; set; }
        public int AttachmentMetadataId { get; set; }
        public AttachmentMetadata AttachmentMetadata { get; set; }
        public byte[] Data { get; set; }


    }
}

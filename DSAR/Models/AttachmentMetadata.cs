using DSAR.Models;

public class AttachmentMetadata
{
    public int Id { get; set; }

    public int FormDataId { get; set; }
    public FormData FormData { get; set; }

    public AttachmentData AttachmentData { get; set; }

    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public long FileSize { get; set; }
    public string FieldName { get; set; } // Add this

}

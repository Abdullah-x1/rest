
using DSAR.Models;

namespace DSAR.Models
{
    public class SnapshotDescriptionEntry
    {
        public int Id { get; set; }
        public int SnapshotFormDataId { get; set; }
        public SnapshotFormData SnapshotFormData { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }
    }
}
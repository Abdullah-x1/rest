
namespace DSAR.Models
{
    public class SnapshotAuthorizedContactEntry
    {
        public int Id { get; set; }

        public int SnapshotFormDataId { get; set; }

        public SnapshotFormData SnapshotFormData { get; set; }

        public string? ApprovedCities { get; set; }            // المدن الموافقة على النموذج
        public string? SectorRepresentative { get; set; }      // ممثل القطاع الموافق على النموذج
        public string? SectorRepresentativeTitle { get; set; } // منصب ممثل القطاع
    
    }

}

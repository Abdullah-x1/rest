using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class AuthorizedContactEntry
    {
    
        public int Id { get; set; }

        public string? ApprovedCities { get; set; }            // المدن الموافقة على النموذج
        public string? SectorRepresentative { get; set; }      // ممثل القطاع الموافق على النموذج
        public string? SectorRepresentativeTitle { get; set; } // منصب ممثل القطاع

        public int RequestId { get; set; }
        public FormData FormData { get; set; }
    }
}

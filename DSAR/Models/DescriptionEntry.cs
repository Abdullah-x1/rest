
using DSAR.Models;

namespace DSAR.Models
{
    public class DescriptionEntry
    {
        public int Id { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }

        public int RequestId { get; set; }
        public FormData? FormData { get; set; }  // navigation property
    }

}



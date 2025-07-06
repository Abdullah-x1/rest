
namespace DSAR.Models
{
    public class MultiRowFormViewModel
    {
        public int RequestId { get; set; }
        public List<DescriptionEntry> Descriptions { get; set; } = new();
        public string ReturnUrl { get; set; } // Add this property
        public int FormId { get; set; }  // Add this property

    }
}
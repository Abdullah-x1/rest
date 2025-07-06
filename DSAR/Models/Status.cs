namespace DSAR.Models
{
    public class Status
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public ICollection<History> Histories { get; set; }

    }
}

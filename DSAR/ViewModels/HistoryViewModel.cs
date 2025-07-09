namespace DSAR.ViewModels
{
    public class HistoryViewModel
    {
        public string StatusName { get; set; }
        public string DepartmentName { get; set; }
        public string RequestNumber { get; set; }
        public int LevelId { get; set; }
        public int RequestId { get; set; }

        public DateTime CreationDate { get; set; }
        public string LevelName { get; set; }

        public string RoleName { get; set; }
        public string Information { get; set; }
    }
}

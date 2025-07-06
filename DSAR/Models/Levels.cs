using System.ComponentModel.DataAnnotations;

namespace DSAR.Models
{
    public class Levels
    {
        [Key]
        public int LevelId { get; set; }
        public string LevelName { get; set; }
    }
}

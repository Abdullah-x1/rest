
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public DateTime CreationDate { get; set; }

        [ForeignKey("Request")]
        public int RequestId { get; set; }

        // navigation
        public Status Status { get; set; }
        public User User { get; set; }
        public FormData FormData { get; set; }
    }
}

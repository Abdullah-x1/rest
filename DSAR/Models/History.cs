using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DSAR.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required, ForeignKey(nameof(Levels))]
        public int LevelId { get; set; }
        public Levels Levels { get; set; }

        [Required, ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public Status Status { get; set; }

        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required, ForeignKey(nameof(FormData))]
        public int RequestId { get; set; }
        public FormData FormData { get; set; }

        [Required, ForeignKey(nameof(Role))]
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }

        public string Information { get; set; }
    }
}

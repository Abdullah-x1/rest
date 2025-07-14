using DSAR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DSAR.Models
{
    public class SnapshotFormData
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "nvarchar(max)")]
        public string FormDataJson { get; set; }
        public bool TermsAccepted { get; set; } = false;

        public ICollection<SnapshotAttachmentMetadata> Attachments { get; set; } = new List<SnapshotAttachmentMetadata>();
        public ICollection<SnapshotDescriptionEntry> Descriptions { get; set; } = new List<SnapshotDescriptionEntry>();

        public FormData GetFormData(JsonSerializerOptions options)
        {
            try
            {
                return JsonSerializer.Deserialize<FormData>(FormDataJson, options) ?? new FormData();
            }
            catch
            {
                return new FormData();
            }
        }

        public void SetFormData(FormData formData, JsonSerializerOptions options)
        {
            FormDataJson = JsonSerializer.Serialize(formData, options);
        }
    }
}
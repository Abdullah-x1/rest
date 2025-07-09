using DSAR.Models;
using DSAR.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSAR.Repository
{
    public interface iFormRepository
    {
        // Main form operations
        Task<IEnumerable<FormData>> GetAll();
        Task<FormData> GetById(int id);
        Task Create(FormData data);
        Task SaveAsync();
        Task CreateWithAttachment(FormData data);

        // Snapshot-based step handlers
        Task<RequestViewModel> GetCurrentFormData();
        // In iFormRepository.cs
        Task<bool> HandleStep1Data(RequestViewModel data, IFormFile attachment);
        Task<bool> HandleStep2Data(RequestViewModel data, IFormFile attachment1, IFormFile attachment2);
        Task<FormData> HandleStep3Data(RequestViewModel data, string UserId);
        Task<(bool isSaved, string workflowName, string uploadsName, string documentsName)> HandleStep4Data(
            RequestViewModel data,
            IFormFile workflowFile,
            IFormFile uploadsRequiredFile,
            IFormFile documentsFile);

        // Description handling
        Task<AttachmentData> GetAttachmentById(int attachmentId); // Add this

        Task<bool> HandleDescriptions(List<DescriptionEntry> descriptions); Task<List<DescriptionEntry>> GetDescriptions();
        Task ClearCurrentSnapshot();

        Task<List<DescriptionEntry>> GetDescriptionsByRequestId(int requestId);
        Task<SnapshotAttachmentData> GetSnapshotAttachmentById(int id);


    }
}
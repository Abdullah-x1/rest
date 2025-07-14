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
        Task<bool> HandleStep1Data(RequestViewModel data, List<IFormFile> attachments);
        Task<bool> HandleStep2Data(RequestViewModel data, List<IFormFile> attachments2, List<IFormFile> attachments3);
        Task<FormData> HandleStep3Data(RequestViewModel data);
        Task<(bool isSaved, string workflowName, string uploadsName, string documentsName)> HandleStep4Data(
         RequestViewModel data,
         List<IFormFile> workflowFiles,
         List<IFormFile> uploadsRequiredFiles,
         List<IFormFile> documentsFiles);


        // Description handling
        Task<AttachmentData> GetAttachmentById(int attachmentId); // Add this

        Task<bool> HandleDescriptions(List<DescriptionEntry> descriptions); Task<List<DescriptionEntry>> GetDescriptions();
        Task ClearCurrentSnapshot();

        Task<List<DescriptionEntry>> GetDescriptionsByRequestId(int requestId);
        Task<SnapshotAttachmentData> GetSnapshotAttachmentById(int id);
        Task<SnapshotFormData> GetCurrentSnapshotAsync();
        Task AcceptTermsAsync();


    }
}
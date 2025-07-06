using DSAR.Models;
using DSAR.ViewModels;

namespace DSAR.Interfaces
{
    public interface ICaseStudyRepository
    {
        void CreateCaseStudy(string analyzerUserId, int requestId);
        void Update(CaseStudy caseStudy);
        Task<List<CaseStudyViewModel>> GetCaseStudiesByUserIdAsync(string userId);
        Task<CaseStudy?> GetCaseStudyByUserIdAsync(string userId, int requestId);
        Task<CaseStudy?> GetByIdAsync(int caseId);
        Task<CaseStudy?> GetByRequestIdAsync(int requestId);
        Task AddAttachmentAsync(int caseStudyId, IFormFile file);
        Task<List<CaseStudyAttachmentMetadata>> GetAttachmentsByCaseStudyIdAsync(int caseStudyId);
        Task<CaseStudyAttachmentData?> GetAttachmentDataByIdAsync(int id);
    }
}

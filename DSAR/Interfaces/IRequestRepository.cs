using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace DSAR.Interfaces
{
    public interface IRequestRepository
    {
        Task<List<FormData>> GetAllWithUserAsync();
        FormData GetById(int Id);
        Task<FormData> GetByIdAsync(int requestId);

        public FormData Create(RequestViewModel viewModel);
         Task<FormData> CreateForSectionManagerAsync(RequestViewModel viewModel, User currentUser);

        void Update(FormData user);
        void Delete(string Id);
        void Save();
         
         Task<List<FormData>> GetRequestsForManagerDepartmentAsync(string managerId, int userSectionId);
        Task<List<FormData>> GetRequestsFromManagersInDepartmentAsync(string bigManagerId, int userDepartmentId);
        Task<List<FormData>> GetRequestsForITManager(string ITManagerId, int userDepartmentId);
        Task<List<FormData>> GetRequestsForApplicationManager(string ApplicationManagerId, int userDepartmentId);
        Task<List<FormData>> GetRequestsForAnalyzer(string AnalyzerId, int userDepartmentId);
        Task<List<FormData>> GetRequestsForCaseStudyUsers(string CaseStudyId, int userDepartmentId);
        Task<List<FormData>> GetRequestsByUserId(string UserId);
        Task<List<FormData>> GetRequestsStillInProcessByUserId(string UserId);

        Task<bool> SendEmailAsync(RequestViewModel request, User currentUser);


        //NewFrom




    }
}

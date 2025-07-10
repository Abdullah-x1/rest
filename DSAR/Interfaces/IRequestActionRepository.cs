using DSAR.Models;
using DSAR.ViewModels;

namespace DSAR.Interfaces
{
    public interface IRequestActionRepository
    {
        public IEnumerable<RequestActions> GetAllByUserId(string userId);
        IEnumerable<RequestActions> GetAll();
        RequestActions GetById(string Id);
        //void Create(RequestActions request);
        void UpdateLevel(RequestActions request);
        void Delete(string Id);
        void Save();
        public RequestActions Create(RequestViewModel viewModel,User currentUser, FormData request);
        public RequestActions CreateSectionManager(RequestViewModel viewModel, User currentUser, FormData request);
        public RequestActions CreateDepartmentManager(RequestViewModel viewModel, User currentUser, FormData request);
        public RequestActions CreateITManager(RequestViewModel viewModel, User currentUser, FormData request);

        Task<RequestActions> GetRequestActionByRequestIdAsync(int requestId);
         Task<RequestActions?> GetByIdAsync(int id);

        Task<List<RequestActions>> GetCompeleteRequestsByUserId(string UserId);
        Task<List<RequestActions>> GetRequestsStillInProcessByUserId(string UserId);

        Task<bool> ProtectViewPages(int Id, User currentUser, FormData form, RequestActions requestActions);



    }
}

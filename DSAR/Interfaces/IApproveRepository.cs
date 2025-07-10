using DSAR.Models;
using DSAR.ViewModels;

namespace DSAR.Interfaces
{

    public interface IApproveRepository
    {
        Task ApproveRequestByDepartmentManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request);
        Task ApproveRequestBySectionManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request);
        Task ApproveRequestByITManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request);
        Task ApproveRequestByApplicationManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request);

        Task<bool> ProtectApprovePage(int requestId, User currentUser, FormData form, RequestActions requestAction);
    }
}

using Azure.Core;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DSAR.Repositories
{
    public class ApproveRepository : IApproveRepository
    {
        private readonly IRequestActionRepository _requestActionRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICaseStudyRepository _caseStudyRepository;
        //private readonly UserManager<User> _userManager;
        //private readonly iFormRepository _formRepo;
        private readonly IAppHistoryRepository _historyRepository;
        private readonly UserManager<User> _userManager;


        public ApproveRepository(IRequestActionRepository requestActionRepository, IRequestRepository requestRepository, IUserRepository userRepository, ICaseStudyRepository caseStudyRepository, IAppHistoryRepository historyRepository, UserManager<User> userManager)
        {
            _requestActionRepository = requestActionRepository;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            //_userManager = userManager;
            _caseStudyRepository = caseStudyRepository;
            //_formRepo = formRepo;
            _historyRepository = historyRepository;
            _userManager = userManager;
        }

        public async Task ApproveRequestByDepartmentManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request)
        {

            var action = await _requestActionRepository.GetByIdAsync(actionId);
            //if (action == null) return RedirectToAction("Main", "Account");

            if (decision == "approve")
            {
                action.StatusId = 2; // Approved
                action.LevelId = 3;
                request.DepartmentNotes = model.DepartmentNotes;

                //history

                const int initialStatusId = 2; // "in progress" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    3,
                    "Request approved by "+currentUser.FirstName+" "+currentUser.LastName
                );
                //history
            }
            else if (decision == "decline")
            {
                action.StatusId = 4; // You can define 99 = Declined in your status table
                action.LevelId = 9;   // Stays the same
                request.DepartmentNotes = model.DepartmentNotes; // Optional
                                                                 //history
                const int initialStatusId = 4; // "rejected" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    9,
                    "Request Rejected by " + currentUser.FirstName + " " + currentUser.LastName
                );
                //history
            }

            _requestActionRepository.UpdateLevel(action);
            _requestRepository.UpdateNotes(request);

        }

        public async Task ApproveRequestBySectionManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request)
        {

            var action = await _requestActionRepository.GetByIdAsync(actionId);
            // if (action == null) return RedirectToAction("Main", "Account");
            if (decision == "approve")
            {
                action.StatusId = 2;
                action.LevelId = 2;
                request.SectionNotes = model.SectionNotes;
                //history
                const int initialStatusId = 2; // "in progress" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    2,
                    "Request approved by " + currentUser.FirstName + " " + currentUser.LastName
                );
                //history
            }
            else if (decision == "decline")
            {
                action.StatusId = 4; // You can define 99 = Declined in your status table
                action.LevelId = 9;   // Stays the same
                request.SectionNotes = model.SectionNotes; // Optional
                                                           //history
                const int initialStatusId = 4; // "rejected" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    9,
                    "Request Rejected by " + currentUser.FirstName + " " + currentUser.LastName
                );
                //history
            }

            _requestActionRepository.UpdateLevel(action);

            if (await _userManager.IsInRoleAsync(currentUser, "DepartmentManager"))
            {
                request.DepartmentNotes = model.DepartmentNotes;
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "SectionManager"))
            {
                request.SectionNotes = model.SectionNotes;
            }
            _requestRepository.UpdateNotes(request);
        }
        public async Task ApproveRequestByITManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request)
        {
                var action = await _requestActionRepository.GetByIdAsync(actionId);
                if (decision == "approve")
                {
                    if (action.LevelId == 3)
                    {
                        action.StatusId = 2;
                        action.LevelId = 4;
                        request.ITNotes = model.ITNotes;
                        _requestActionRepository.UpdateLevel(action);

                        //history
                        const int initialStatusId = 2; // "in progress" status
                        await _historyRepository.CreateHistoryAsync(
                            currentUser,
                            request,
                            initialStatusId,
                            4,
                            "Request approved initially by " + currentUser.FirstName + " " + currentUser.LastName
                        );
                        //history
                    }
                    else if (action.LevelId == 7)
                    {
                        action.StatusId = 3;
                        action.LevelId = 8;
                        request.ITNotes = model.ITNotes;
                    _requestActionRepository.UpdateLevel(action);
                        //history
                        const int initialStatusId = 3; // "approved" status
                        await _historyRepository.CreateHistoryAsync(
                            currentUser,
                            request,
                            initialStatusId,
                            8,
                            "Request approved by " + currentUser.FirstName + " " + currentUser.LastName
                        );
                        //history
                    }
                }
                else if (decision == "decline")
                {
                    action.StatusId = 4; // You can define 99 = Declined in your status table
                    action.LevelId = 9;   // Stays the same
                    request.ITNotes = model.ITNotes;
                //history
                const int initialStatusId = 4; // "rejected" status
                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        9,
                        "Request Rejected by " + currentUser.FirstName + " " + currentUser.LastName
                    );
                    //history
                }
                // ✅ Optional: add note for administration
                // request.AdministrationNote = notes;
                _requestRepository.UpdateNotes(request);
        }
        public async Task ApproveRequestByApplicationManager(RequestViewModel model, int actionId, int requestId, string decision, User currentUser, FormData request)
        {
          
                var action = await _requestActionRepository.GetByIdAsync(actionId);
                //if (action == null) return RedirectToAction("Main", "Account");
                

                if (decision == "approve")
                {
                    action.LevelId = 7;
                    request.ApplicationNotes = model.ApplicationNotes;
                    _requestActionRepository.UpdateLevel(action);

                    // ✅ Optional: add note for administration
                    // request.AdministrationNote = notes;
                    _requestRepository.UpdateNotes(request);
                    //history
                    const int initialStatusId = 2; // "in progress" status
                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        7,
                        "Request approved by " + currentUser.FirstName + " " + currentUser.LastName
                    );
                    //history
                }
                else if (decision == "decline")
                {
                    action.StatusId = 4; // You can define 99 = Declined in your status table
                    action.LevelId = 9;   // Stays the same
                    request.ApplicationNotes = model.ApplicationNotes;
                _requestRepository.UpdateNotes(request);
                    //history
                    const int initialStatusId = 4; // "rejected" status
                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        9,
                        "Request Rejected by " + currentUser.FirstName + " " + currentUser.LastName
                    );
                    //history
                }

        }
        public async Task<bool> ProtectApprovePage(int requestId, User currentUser, FormData request, RequestActions requestAction)
        {
            int currentDepartmentId = currentUser.DepartmentId;
            int requestDepartmentId = requestAction.DepartmentId;

            // Check manager roles
            bool isSectionManager = await _userManager.IsInRoleAsync(currentUser, "SectionManager");
            bool isDepartmentManager = await _userManager.IsInRoleAsync(currentUser, "DepartmentManager");
            bool isITManager = await _userManager.IsInRoleAsync(currentUser, "ITManager");
            bool isApplicationManager = await _userManager.IsInRoleAsync(currentUser, "ApplicationManager");
            bool isAnalyzer = await _userManager.IsInRoleAsync(currentUser, "Analyzer");

            // ✅ Check if manager is allowed to access based on department
            if ((isSectionManager || isDepartmentManager || isITManager || isApplicationManager || isAnalyzer)
                && requestDepartmentId != currentDepartmentId)
            {
                return false;
            }

            // ✅ Allow only the correct manager level to access
            if (isSectionManager && requestAction.LevelId != 1)
                return false;

            if (isDepartmentManager && requestAction.LevelId != 2)
                return false;

            if (isITManager && !(requestAction.LevelId == 3 || requestAction.LevelId == 7))
                return false;

            if (isApplicationManager && !(requestAction.LevelId == 4 || requestAction.LevelId == 6))
                return false;

            if (isAnalyzer && requestAction.LevelId != 5)
                return false;

            // Final fallback — no matching role
            if (!isSectionManager && !isDepartmentManager && !isITManager && !isApplicationManager && !isAnalyzer)
                return false;

            // If all checks pass, return true
            return true;
        }

    }
}

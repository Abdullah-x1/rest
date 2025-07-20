using Azure.Core;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.Repositories;
using DSAR.Repository;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DSAR.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestActionRepository _requestActionRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICaseStudyRepository _caseStudyRepository;
        private readonly UserManager<User> _userManager;
        private readonly iFormRepository _formRepo;
        private readonly IAppHistoryRepository _historyRepository;
        private readonly IApproveRepository _approveRepository;


        public RequestController(IRequestActionRepository requestActionRepository, IRequestRepository requestRepository, IUserRepository userRepository, ICaseStudyRepository caseStudyRepository, UserManager<User> userManager, iFormRepository formRepo, IAppHistoryRepository historyRepository, IApproveRepository approveRepository)
        {
            _requestActionRepository = requestActionRepository;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _caseStudyRepository = caseStudyRepository;
            _formRepo = formRepo;
            _historyRepository = historyRepository;
            _approveRepository = approveRepository;
        }

        public async Task<IActionResult> MyRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            var requestAction = await _requestActionRepository.GetRequestsStillInProcessByUserId(userId); // Filtered list
            var viewModel = requestAction.Select(r => new RequestViewModel
            {
                RequestId = r.RequestId,
                FirstName = r.User?.FirstName,
                LastName = r.User?.LastName,
                StatusName = r.Status?.StatusName,
                DepartmentName = r.Department?.DepartmentName,
                RequestNumber = r.FormData.RequestNumber,
                LevelId = r.LevelId,
                ActionId = r.ActionId,

            }).ToList();

            return View(viewModel);
        }
        [Authorize]
        public IActionResult Request()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Request(RequestViewModel data)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            FormData request;
            bool emailResponse;
            const int initialStatusId = 1; // "new submission" status
            if (currentUser.TermsAccepted == true)
            {


                if (await _userManager.IsInRoleAsync(currentUser, "SectionManager"))
                {
                    data.UserId = currentUser.Id;
                    request = await _formRepo.HandleStep3Data(data, currentUser.UserId);
                    _requestActionRepository.CreateSectionManager(data, currentUser, request);

                    emailResponse = await _requestRepository.SendEmailAsync(data, currentUser);
                    if (!emailResponse)
                    {
                        // Handle false response  
                    }
                    ////////////////////history///////////////////////////////////////

                    await _historyRepository.CreateHistoryAsync(

                        currentUser,
                        request,
                        initialStatusId,
                        1,
                        "Request submitted"
                    );

                    ////////////////////history///////////////////////////////////////
                    return RedirectToAction("Main", "Account");
                }
                else if (await _userManager.IsInRoleAsync(currentUser, "DepartmentManager"))
                {
                    data.UserId = currentUser.Id;
                    request = await _formRepo.HandleStep3Data(data, currentUser.UserId);
                    _requestActionRepository.CreateDepartmentManager(data, currentUser, request);

                    emailResponse = await _requestRepository.SendEmailAsync(data, currentUser);
                    if (!emailResponse)
                    {
                        // Handle false response  
                    }

                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        1,
                        "Request submitted"
                    );
                    return RedirectToAction("Main", "Account");
                }
                else if (await _userManager.IsInRoleAsync(currentUser, "ITManager"))
                {
                    data.UserId = currentUser.Id;
                    request = await _formRepo.HandleStep3Data(data, currentUser.UserId);
                    _requestActionRepository.CreateITManager(data, currentUser, request);

                    emailResponse = await _requestRepository.SendEmailAsync(data, currentUser);
                    if (!emailResponse)
                    {
                        // Handle false response  
                    }
                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        1,
                        "Request submitted"
                    );
                    return RedirectToAction("Main", "Account");
                }
                else if (await _userManager.IsInRoleAsync(currentUser, "ApplicationManager"))
                {
                    data.UserId = currentUser.Id;
                    request = await _formRepo.HandleStep3Data(data, currentUser.UserId);
                    _requestActionRepository.CreateITManager(data, currentUser, request);

                    emailResponse = await _requestRepository.SendEmailAsync(data, currentUser);
                    if (!emailResponse)
                    {
                        // Handle false response  
                    }
                    await _historyRepository.CreateHistoryAsync(
                        currentUser,
                        request,
                        initialStatusId,
                        1,
                        "Request submitted"
                    );
                    return RedirectToAction("Main", "Account");
                }
                else if (await _userManager.IsInRoleAsync(currentUser, "User"))
                    data.UserId = currentUser.Id;
                request = await _formRepo.HandleStep3Data(data, currentUser.UserId);

                _requestActionRepository.Create(data, currentUser, request);

                emailResponse = await _requestRepository.SendEmailAsync(data, currentUser);
                if (!emailResponse)
                {
                    // Handle false response  
                }
                ////////////////////history///////////////////////////////////////

                // "new submission" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    1,
                    "Request submitted"
                );

                ////////////////////history///////////////////////////////////////
                return RedirectToAction("Main", "Account");
            }
            else if(currentUser.TermsAccepted == false)
            {
                TempData["Error"] = "Please accept the terms and conditions before submitting a request.";
                return RedirectToAction("Step3","Request");
            }
            return RedirectToAction("Main", "Account");
        }
        //New Form



        public async Task<IActionResult> OrderPage()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser.Id;

            List<RequestActions> requests;


            if (await _userManager.IsInRoleAsync(currentUser, "DepartmentManager"))
            {
                requests = await _requestActionRepository.GetRequestsForDepartmentManagerAsync(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "SectionManager"))
            {
                requests = await _requestActionRepository.GetRequestsForSectionManagerAsync(userId, currentUser.SectionId,currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "ITManager"))
            {
                requests = await _requestActionRepository.GetRequestsForITManager(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Analyzer"))
            {
                requests = await _requestActionRepository.GetRequestsForAnalyzer(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "ApplicationManager"))
            {
                requests = await _requestActionRepository.GetRequestsForApplicationManager(userId, currentUser.DepartmentId);
            }
            else
            {
                return RedirectToAction("Account", "Main");
            }


            var viewModels = requests.Select(r => new RequestViewModel
            {
                RequestId = r.RequestId,
                LevelId = r.LevelId,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                DepartmentName = r.User.Department.DepartmentName,
                RequestNumber = r.FormData.RequestNumber,
                ActionId = r.ActionId,
                StatusName = r.Status?.StatusName,
            }).ToList();
            return View(viewModels);
        }

        public async Task<IActionResult> CombinedView(int id, int actionId)
        {

            var histories = await _historyRepository.GetHistoriesByRequestIdAsync(id);

            var historyVm = histories.Select(h => new HistoryViewModel
            {
                CreationDate = h.CreationDate,
                LevelName = h.Levels.LevelName,
                StatusName = h.Status.StatusName,
                RoleName = h.Role.Name,
                Information = h.Information,
                Notes = h.Role.Name switch
                {
                    "SectionManager" => h.SectionNotes ?? "",
                    "DepartmentManager" => h.DepartmentNotes ?? "",
                    "ITManager" => h.ITNotes ?? "",
                    "ApplicationManager" => h.ApplicationNotes ?? "",
                    _ => " لا يوجد "
                }
            }).ToList();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var form = await _formRepo.GetById(id);
            if (form == null) return NotFound();

            var request = _requestRepository.GetById(id);
            if (request == null) return NotFound();

            var requestAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(request.RequestId);
            if (requestAction == null) return NotFound();

            var allowed = await _requestActionRepository.ProtectViewPages(id, currentUser, request, requestAction);
            if (!allowed) return RedirectToAction("Main", "Account");

            var history = await _historyRepository.GetHistoriesByRequestIdAsync(id);
            var descriptions = await _formRepo.GetDescriptionsByRequestId(id);
            var contacts = await _formRepo.GetAuthorizedContactsByRequestId(id);

            var viewModel = new RequestViewModel
            {
                RequestId = form.RequestId,
                ServiceName = form.ServiceName,
                ServiceTypeAndLocation = form.ServiceTypeAndLocation,
                ServiceDescription = form.ServiceDescription,
                HasDependency = form.HasDependency,
                DependencyDetails = form.DependencyDetails,
                ProcedureNumber = form.ProcedureNumber,

                Name = form.Name,
                Email = form.Email,
                RepeatLimit = form.RepeatLimit,
                DepartmentName = form.Department?.DepartmentName,
                Fees = form.Fees,
                TargetAudience = form.TargetAudience,
                DepName = form.Departments,
                ExpectedOutput1 = form.ExpectedOutput1,
                ExpectedOutput2 = form.ExpectedOutput2,
                ApprovedTemplate = form.ApprovedTemplate,
                DetailedInfo = form.DetailedInfo,
                RequiredConditions = form.RequiredConditions,

                Workflow = form.Workflow,
                UploadsRequired = form.UploadsRequired,
                Documents = form.Documents,
                Timeline = form.Timeline,
                SystemNeeded = form.SystemNeeded,

                Attachments = form.Attachments?.ToList(),
                Descriptions = descriptions?.ToList() ?? new List<DescriptionEntry>(),
                AuthorizedContacts = contacts?.ToList() ?? new List<AuthorizedContactEntry>(),

                History = history.Select(h => new HistoryViewModel
                {
                    CreationDate = h.CreationDate,
                    LevelName = h.Levels.LevelName,
                    StatusName = h.Status.StatusName,
                    RoleName = h.Role.Name,
                    Information = h.Information
                }).ToList(),

                ActionId = requestAction?.ActionId ?? 0,
                LevelId = requestAction?.LevelId ?? 0
            };

            return View(viewModel);
        }
        public async Task<IActionResult> StepDescriptionsView(int id, int actionId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var form = await _formRepo.GetById(id);
            if (form == null) return NotFound();
            var request = _requestRepository.GetById(id);
            if (request == null) return NotFound();

            var requestAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(request.RequestId);
            if (requestAction == null) return NotFound();


            var allowed = await _requestActionRepository.ProtectViewPages(id, currentUser, request, requestAction);
            if (!allowed)
            {
                // Redirect to Main page if not allowed
                return RedirectToAction("Main", "Account");
            }

            var formData = await _formRepo.GetDescriptionsByRequestId(id);
            if (formData == null) return NotFound();

            // Create the proper view model  
            var viewModel = new RequestViewModel
            {
                RequestId = form.RequestId,
                Descriptions = formData?.ToList() ?? new List<DescriptionEntry>(),
                ActionId = requestAction?.ActionId ?? 0, // Fix: Use null-coalescing operator to handle null reference  
                LevelId = requestAction?.LevelId ?? 0,
            };

            return View(viewModel);
        }


        // STEP 1 - GET
        public async Task<IActionResult> Step1()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count > 0)
            {
                TempData["Error"] = "لديك طلب معلق. يُرجى الانتظار حتى اكتماله قبل إرسال طلب جديد.";
                return RedirectToAction("Main", "Account");
            }

            var snapshot = await _formRepo.GetCurrentSnapshotAsync(); // You'll create this method below
            ViewBag.ShowTermsPopup = snapshot?.TermsAccepted == false;

            return View(await _formRepo.GetCurrentFormData());
        }




        [HttpPost]
        public async Task<IActionResult> Step1(RequestViewModel data, List<IFormFile> Attachment1, string action)
        {
            if (action == "save")
            {
                bool isSaved = await _formRepo.HandleStep1Data(data, Attachment1);
                if (isSaved)
                    return Json(new { success = true, message = "تم الحفظ بنجاح", attachmentName = data.Attachment1Name });
                else
                    return Json(new { success = false, message = "لم تقم بتعديل أي حقل أو إرفاق ملف." });
            }
            else if (action == "next")
            {
                await _formRepo.HandleStep1Data(data, Attachment1); // ensure saved
                return RedirectToAction("Step2");
            }
            var vm = await _formRepo.GetCurrentFormData();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptTerms()
        {
            try
            {
                await _formRepo.AcceptTermsAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Unable to accept terms");
            }
        }


        // STEP 2 - GET
        public async Task<IActionResult> Step2()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();
            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count! > 0)
            {
                TempData["Error"] = "You already have a pending request. Please wait until it is completed before submitting a new one.";
                return RedirectToAction("Main", "Account");
            }
            return View(await _formRepo.GetCurrentFormData());
        }

       
        [HttpPost]
        public async Task<IActionResult> Step2(RequestViewModel data, List<IFormFile> Attachment2, List<IFormFile> Attachment3, string action)
        {
            if (action == "save")
            {
                bool isSaved = await _formRepo.HandleStep2Data(data, Attachment2, Attachment3);
                if (isSaved)
                    return Json(new { success = true, message = "تم الحفظ بنجاح" });
                else
                    return Json(new { success = false, message = "لم تقم بتعديل أي حقل أو إرفاق ملف." });
            }
            else if (action == "next")
            {
                await _formRepo.HandleStep2Data(data, Attachment2, Attachment3); // save before navigating
                return RedirectToAction("Step4");
            }

            var vm = await _formRepo.GetCurrentFormData();
            return View(vm);
        }

        
        // STEP 3 - GET
        public async Task<IActionResult> Step3()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();
            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count! > 0)
            {
                TempData["Error"] = "You already have a pending request. Please wait until it is completed before submitting a new one.";
                return RedirectToAction("Main", "Account");
            }
                return View(await _formRepo.GetCurrentFormData());
        }

        // STEP 3 - POST

        //public async Task<IActionResult> Step3(RequestViewModel data)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    data.Id = currentUser.Id;

        //    await _formRepo.HandleStep3Data(data, currentUser.UserId);
        //    return RedirectToAction("OrderPage");

        //}

        // STEP 4 - GET
        [HttpGet]
        public async Task<IActionResult> Step4()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();
            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count! > 0)
            {
                TempData["Error"] = "You already have a pending request. Please wait until it is completed before submitting a new one.";
                return RedirectToAction("Main", "Account");
            }
            return View(await _formRepo.GetCurrentFormData());
        }

        // STEP 4 - POST
       
        [HttpPost]
        public async Task<IActionResult> Step4(
    RequestViewModel data,
    List<IFormFile> WorkflowFile,
    List<IFormFile> UploadsRequiredFile,
    List<IFormFile> DocumentsFile,
    string action)

        {
            if (action == "save")
            {
                var (isSaved, workflowName, uploadsName, documentsName) = await _formRepo.HandleStep4Data(
                    data,
                    WorkflowFile,
                    UploadsRequiredFile,
                    DocumentsFile
                );

                if (isSaved)
                {
                    return Json(new
                    {
                        success = true,
                        message = "تم الحفظ بنجاح",
                        workflowName,
                        uploadsName,
                        documentsName
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "لم تقم بتعديل أي حقل أو إرفاق ملف."
                    });
                }
            }
            else if (action == "next")
            {
                await _formRepo.HandleStep4Data(
                    data,
                    WorkflowFile,
                    UploadsRequiredFile,
                    DocumentsFile
                );

                return RedirectToAction("StepAuthorizedContacts");
            }

            var vm = await _formRepo.GetCurrentFormData();
            return View(vm);
        }

        // STEP DESCRIPTIONS - GET
        [HttpGet]
        public async Task<IActionResult> StepDescriptions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();
            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count! > 0)
            {
                TempData["Error"] = "You already have a pending request. Please wait until it is completed before submitting a new one.";
                return RedirectToAction("Main", "Account");
            }
            var descriptions = await _formRepo.GetDescriptions();
            if (descriptions.Count == 0)
            {
                descriptions.Add(new DescriptionEntry());
            }
            // Example mapping: create new DescriptionEntry objects (or a custom view model if you have one)
            var mappedDescriptions = descriptions.Select(d => new DescriptionEntry
            {

                Description1 = d.Description1,
                Description2 = d.Description2,

            }).ToList();
            var vm = await _formRepo.GetCurrentFormData();
            vm.Descriptions = descriptions;
            return View(vm);
        }

        // STEP DESCRIPTIONS - POST
        [HttpPost]
        public async Task<IActionResult> StepDescriptions(RequestViewModel model, string action)
        {
            if (action == "save")
            {
                await _formRepo.HandleDescriptions(model.Descriptions);
                return Json(new { success = true, message = "تم الحفظ بنجاح" });
            }
            else if (action == "next")
            {
                await _formRepo.HandleDescriptions(model.Descriptions);
                return RedirectToAction("Step3");
            }

            return View(model); // fallback
        }


        [HttpGet]
        public async Task<IActionResult> StepAuthorizedContacts()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var requests = await _requestActionRepository.GetRequestsStillInProcessByUserId(currentUser.Id);
            if (requests.Count > 0)
            {
                TempData["Error"] = "You already have a pending request. Please wait until it is completed before submitting a new one.";
                return RedirectToAction("Main", "Account");
            }

            var contactsFromRepo = await _formRepo.GetAuthorizedContacts();

            if (contactsFromRepo.Count == 0)
            {
                contactsFromRepo.Add(new AuthorizedContactEntry());
            }

            // Map repo model to clean ViewModel objects
            var mappedContacts = contactsFromRepo.Select(c => new AuthorizedContactEntry
            {
                Id = c.Id,
                ApprovedCities = c.ApprovedCities?.Trim(),
                SectorRepresentative = c.SectorRepresentative?.Trim(),
                SectorRepresentativeTitle = c.SectorRepresentativeTitle?.Trim(),
                RequestId = c.RequestId
            }).ToList();

            var vm = await _formRepo.GetCurrentFormData();
            vm.AuthorizedContacts = mappedContacts;

            return View(vm);
        }



        [HttpPost]
        public async Task<IActionResult> StepAuthorizedContacts(RequestViewModel model, string action)
        {

            if (action == "save")
            {
                await _formRepo.HandleAuthorizedContacts(model.AuthorizedContacts ?? new List<AuthorizedContactEntry>());
                return Json(new { success = true, message = "تم الحفظ بنجاح" });
            }
            else if (action == "next")
            {
                await _formRepo.HandleAuthorizedContacts(model.AuthorizedContacts ?? new List<AuthorizedContactEntry>());
                return RedirectToAction("StepDescriptions");
            }

            return View(model); // fallback
        }





        public IActionResult Complete()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            var attachment = await _formRepo.GetAttachmentById(attachmentId);
            if (attachment == null) return NotFound();

            return File(attachment.Data, "application/octet-stream",
                       $"{attachment.AttachmentMetadata.FileName}{attachment.AttachmentMetadata.FileExtension}");
        }
        [HttpGet]
       
        public async Task<IActionResult> Submissions()
        {
            return View(await _formRepo.GetAll());
        }

        // Clear snapshot (for testing/debugging)
        public async Task<IActionResult> ClearSnapshot()
        {
            await _formRepo.ClearCurrentSnapshot();
            return RedirectToAction("Step1");
        }


        public async Task<IActionResult> DownloadSnapshotAttachment(int attachmentId)
        {
            var snapshotAttachment = await _formRepo.GetSnapshotAttachmentById(attachmentId);
            if (snapshotAttachment == null) return NotFound();

            var metadata = snapshotAttachment.SnapshotAttachmentMetadata;

            return File(snapshotAttachment.Data, "application/octet-stream",
                $"{metadata.FileName}{metadata.FileExtension}");
        }


        [HttpPost]
        public async Task<IActionResult> Approve(RequestViewModel model, int actionId, int requestId, string decision)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Account", "Main");

            var request = _requestRepository.GetById(requestId);
            if (request == null)
                return RedirectToAction("Account", "Main");

            if (await _userManager.IsInRoleAsync(currentUser, "DepartmentManager"))
            {
                await _approveRepository.ApproveRequestByDepartmentManager(model, actionId, requestId, decision, currentUser, request);

            }
            else if (await _userManager.IsInRoleAsync(currentUser, "SectionManager"))
            {
                await _approveRepository.ApproveRequestBySectionManager(model, actionId, requestId, decision, currentUser, request);

            }

            else if (await _userManager.IsInRoleAsync(currentUser, "ITManager"))
            {
                await _approveRepository.ApproveRequestByITManager(model, actionId, requestId, decision, currentUser, request);

            }
            else if (await _userManager.IsInRoleAsync(currentUser, "ApplicationManager"))
            {
                await _approveRepository.ApproveRequestByApplicationManager(model, actionId, requestId, decision, currentUser, request);

            }
           
            return RedirectToAction("Main", "Account");

        }

        [HttpGet]
        public async Task<IActionResult> ApprovePageAsync(int requestId, int actionId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var form = await _formRepo.GetById(requestId);
            if (form == null) RedirectToAction("Account", "Main");

            var request = _requestRepository.GetById(requestId);
            if (request == null) RedirectToAction("Account", "Main");

            var requestAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(request.RequestId);
            if (requestAction == null) RedirectToAction("Account", "Main");


            var allowed = await _approveRepository.ProtectApprovePage(requestId, currentUser, request, requestAction);
            if (!allowed)
            {
                // Redirect to Main page if not allowed
                return RedirectToAction("Main", "Account");
            }
            

            var caseStudy = await _caseStudyRepository.GetByRequestIdAsync(requestId);

            List<CaseStudyAttachmentMetadata> attachments = null;
            if (caseStudy != null)
            {
                attachments = await _caseStudyRepository.GetAttachmentsByCaseStudyIdAsync(caseStudy.CaseId);
            }
            ViewBag.Attachments = attachments;

            var action = await _requestActionRepository.GetByIdAsync(actionId);
            if (action == null)
                return RedirectToAction("Account", "Main");

            var viewModel = new RequestViewModel
            {
                RequestId = request.RequestId,
                ActionId = actionId,
                // ServiceName = request.ServiceName,
                SectionNotes = request.SectionNotes,
                DepartmentNotes = request.DepartmentNotes,

                WorkTeam = caseStudy?.WorkTeam,
                Notes = caseStudy?.Notes,
                restriction = caseStudy?.restriction,
                LevelId = action.LevelId,
                ITNotes = request.ITNotes,
                ApplicationNotes = request.ApplicationNotes
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AnalyzerUsers(int requestId)
        {
            var analyzers = await _userManager.GetUsersInRoleAsync("Analyzer");

            var viewModels = analyzers.Select(user => new CaseStudyViewModel
            {
                Id = user.Id,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RequestId = requestId // Use the parameter here
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> ConfirmAnalyzer(int requestId, string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                return NotFound();
            _caseStudyRepository.CreateCaseStudy(userId, requestId);

            var existingAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(requestId);

            if (existingAction != null)
            {

                existingAction.StatusId = 5;
                existingAction.LevelId = 5;
                _requestActionRepository.UpdateLevel(existingAction);
                //history
                const int initialStatusId = 2; // "in progress" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    5,
                    "Request approved initially by ITManager"
                );
                //history
            }

            return RedirectToAction("Main", "Account");
        }

        [Authorize(Roles = "Analyzer")]
        [HttpGet]
        public async Task<IActionResult> CaseStudy(int requestId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();
            
            var caseStudy = await _caseStudyRepository.GetCaseStudyByUserIdAsync(currentUser.Id, requestId);
            if (caseStudy == null) return NotFound();

            var request = await _requestRepository.GetByIdAsync(requestId);

            var requestAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(requestId);

            var allowed = await _requestActionRepository.ProtectCaseStudyPage(requestId, currentUser, request, requestAction);
            if (!allowed) return RedirectToAction("Main", "Account");


            var attachments = await _caseStudyRepository.GetAttachmentsByCaseStudyIdAsync(caseStudy.CaseId);

            var viewModel = new CaseStudyViewModel
            {
                CaseId = caseStudy.CaseId,
                RequestId = caseStudy.RequestId,
                UserId = caseStudy.UserId,
                WorkTeam = caseStudy.WorkTeam,
                restriction = caseStudy.restriction,
                Notes = caseStudy.Notes,
                CreatedAt = caseStudy.CreatedAt,
                SectionNotes = request?.SectionNotes,
                DepartmentNotes = request?.DepartmentNotes,
                ApplicationNotes = request?.ApplicationNotes,
                ITNotes = request?.ITNotes,

                Attachments = attachments.Select(a => new AttachmentViewModel
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileExtension = a.FileExtension
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Analyzer")]
        public async Task<IActionResult> CaseStudy(CaseStudyViewModel model, List<IFormFile> CaseStudyAttachment)
        {
            var caseStudy = await _caseStudyRepository.GetByIdAsync(model.CaseId);
            if (caseStudy == null) RedirectToAction("Account", "Main");


            caseStudy.WorkTeam = model.WorkTeam;
            caseStudy.restriction = model.restriction;
            caseStudy.Notes = model.Notes;
            caseStudy.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            if (CaseStudyAttachment != null && CaseStudyAttachment.Count > 0)
            {
                foreach (var file in CaseStudyAttachment)
                {
                    if (file != null && file.Length > 0)
                    {
                        await _caseStudyRepository.AddAttachmentAsync(caseStudy.CaseId, file);
                    }
                }
            }

            _caseStudyRepository.Update(caseStudy);

            var requestId = caseStudy.RequestId;
            var request = _requestRepository.GetById(requestId);
            if (request == null) RedirectToAction("Account", "Main");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) RedirectToAction("Account", "Main");

            var existingAction = await _requestActionRepository.GetRequestActionByRequestIdAsync(requestId);

            if (existingAction != null)
            {
                existingAction.LevelId = 6;
                existingAction.StatusId = 2;
                _requestActionRepository.UpdateLevel(existingAction);
                // history

                const int initialStatusId = 2; // "in progress" status
                await _historyRepository.CreateHistoryAsync(
                    currentUser,
                    request,
                    initialStatusId,
                    6,
                    "Request studied by Analyzer"
                );
                // history
            }

            return RedirectToAction("Main", "Account");
        }
        public async Task<IActionResult> CancelRequest(int actionId)
        {

            var action = await _requestActionRepository.GetByIdAsync(actionId);
            if (action == null) return RedirectToAction("Main", "Account");

            if (action.LevelId == 1)
            {
                action.StatusId = 3;
                action.LevelId = 9;
                _requestActionRepository.UpdateLevel(action);
            }
            else
            {
                TempData["Error"] = "You can only cancel requests that are at the initial submission stage.";
            }

                return RedirectToAction("Main", "Account");
        }

        public async Task<IActionResult> DownloadCaseStudyAttachment(int attachmentId)
        {
            var attachment = await _caseStudyRepository.GetAttachmentDataByIdAsync(attachmentId);
            if (attachment == null) return NotFound();

            var meta = attachment.CaseStudyAttachmentMetadata;

            return File(attachment.Data, "application/octet-stream", $"{meta.FileName}{meta.FileExtension}");
        }

        public async Task<IActionResult> CompeleteRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            var requestAction = await _requestActionRepository.GetCompeleteRequestsByUserId(userId); // Filtered list
            var viewModel = requestAction.Select(r => new RequestViewModel
            {
                RequestId = r.RequestId,
                StatusName = r.Status?.StatusName,
                DepartmentName = r.Department?.DepartmentName,
                RequestNumber = r.FormData.RequestNumber,
                LevelId = r.LevelId,
                ActionId = r.ActionId

            }).ToList();

            return View(viewModel);
        }

        public IActionResult GetAllRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            var requestAction = _requestActionRepository.GetAllByUserId(userId); // Filtered list
            var viewModel = requestAction.Select(r => new RequestViewModel
            {
                RequestId = r.RequestId,
                StatusName = r.Status?.StatusName,
                DepartmentName = r.Department?.DepartmentName,
                RequestNumber = r.FormData.RequestNumber,
                LevelId = r.LevelId,
                ActionId = r.ActionId

            }).ToList();

            return View(viewModel);
        }
        public async Task<IActionResult> CompeleteRequestManager()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

            var requestAction = await _historyRepository.GetHistroyRequestsByUserId(userId); // Filtered list
            var viewModel = requestAction.Select(r => new HistoryViewModel
            {
                RequestId = r.RequestId,
                StatusName = r.Status?.StatusName,
                 DepartmentName = r.User.Department?.DepartmentName,
                RequestNumber = r.FormData.RequestNumber,
                LevelId = r.LevelId,
                // ActionId = r.ActionId

            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> GetAllRequestsManager()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID
            var currentUser = await _userManager.GetUserAsync(User);

            var historyRequests = await _historyRepository.GetAllHistroyRequestsByUserId(userId); // Filtered list

            List<RequestActions> allRequests;

            
            if (await _userManager.IsInRoleAsync(currentUser, "DepartmentManager"))
            {
                allRequests = await _requestActionRepository.GetRequestsForDepartmentManagerAsync(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "SectionManager"))
            {
                allRequests = await _requestActionRepository.GetRequestsForSectionManagerAsync(userId, currentUser.SectionId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "ITManager"))
            {
                allRequests = await _requestActionRepository.GetRequestsForITManager(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Analyzer"))
            {
                allRequests = await _requestActionRepository.GetRequestsForAnalyzer(userId, currentUser.DepartmentId);
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "ApplicationManager"))
            {
                allRequests = await _requestActionRepository.GetRequestsForApplicationManager(userId, currentUser.DepartmentId);
            }
            else
            {
                return RedirectToAction("Account", "Main");
            }
            var historyRequestIds = new HashSet<int>(historyRequests.Select(h => h.RequestId));

            var newRequests = allRequests.Where(r => !historyRequestIds.Contains(r.RequestId)).ToList();

            var combined = historyRequests.Select(r => new HistoryViewModel
            {
                RequestId = r.RequestId,
                StatusName = r.Status?.StatusName,
                DepartmentName = r.User?.Department?.DepartmentName,
                RequestNumber = (double)(r.FormData?.RequestNumber ?? 0),
                LevelId = r.LevelId
            }).ToList();

            combined.AddRange(newRequests.Select(r => new HistoryViewModel
            {
                RequestId = r.RequestId,
                StatusName = r.Status?.StatusName,
                DepartmentName = r.User?.Department?.DepartmentName,
                RequestNumber = (double)(r.FormData?.RequestNumber ?? 0),
                LevelId = r.LevelId
            }));

            return View(combined);
        }

        [Authorize]
        public async Task<IActionResult> History(int requestId)
        {

            ViewBag.RequestId = requestId;
            var histories = await _historyRepository.GetHistoriesByRequestIdAsync(requestId);

            var historiesVm = histories.Select(h => new HistoryViewModel
            {
                CreationDate = h.CreationDate,
                LevelName = h.Levels.LevelName,
                StatusName = h.Status.StatusName,
                RoleName = h.Role.Name,
                Information = h.Information,

                // pick the correct notes column
                Notes = h.Role.Name switch
                {
                    "SectionManager" => h.SectionNotes ?? "",
                    "DepartmentManager" => h.DepartmentNotes ?? "",
                    "ITManager" => h.ITNotes ?? "",
                    "ApplicationManager" => h.ApplicationNotes ?? "",
                    _ => ""
                }
            })
            .ToList();


            return View(historiesVm);
        }



    }


}

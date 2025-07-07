using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace DSAR.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public RequestRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //public FormData Create(RequestViewModel viewModel)
        //{
        //    var request = new FormData
        //    {

        //        // Field mapping
        //        ServiceName = viewModel.ServiceName,
        //        ServiceType = viewModel.ServiceType,
        //        ServiceDescription = viewModel.ServiceDescription,
        //        HasDependencies = viewModel.HasDependencies,
        //        DependencyExplanation = viewModel.DependencyExplanation,
        //        ProcedureNumber = viewModel.ProcedureNumber,
        //        ProcedureFileUrl = viewModel.ProcedureFileUrl,

        //        // معلومات أولية عن الخدمة
        //        AllowedSubmissionCount = viewModel.AllowedSubmissionCount,
        //        ServiceFee = viewModel.ServiceFee,
        //        TargetAudience = viewModel.TargetAudience,
        //        ResponsibleDepartment = viewModel.ResponsibleDepartment,
        //        AvailableCities = viewModel.AvailableCities,

        //        // صلاحيات المعتمدين
        //        ApproverDepartment = viewModel.ApproverDepartment,
        //        ApproverName = viewModel.ApproverName,
        //        ActionType = viewModel.ActionType,
        //        // Duration = request.Duration,
        //        // DurationUnit = request.DurationUnit,

        //        // مخرجات الخدمة
        //        FinalOutputDescription = viewModel.FinalOutputDescription,
        //        // FinalOutputFile = request.FinalOutputFile,

        //        // النماذج المستخدمة
        //        FieldName = viewModel.FieldName,
        //        FieldType = viewModel.FieldType,
        //        FieldNotes = viewModel.FieldNotes,
        //        IsFieldRequired = viewModel.IsFieldRequired,
        //        FieldEnglishName = viewModel.FieldEnglishName,

        //        // البيانات الاعتمادية
        //        FieldCategories = viewModel.FieldCategories,
        //        FieldRelationships = viewModel.FieldRelationships,

        //        // الشروط العامة
        //        ServiceConditions = viewModel.ServiceConditions,

        //        // معلومات تفصيلية
        //        ExecutionPath = viewModel.ExecutionPath,
        //        // ExecutionAttachment = request.ExecutionAttachment,
        //        ExecutionDuration = viewModel.ExecutionDuration,
        //        // RequiredAttachments = request.RequiredAttachments,
        //        // CityApprovals = request.CityApprovals,

        //        // الربط الخارجي
        //        IntegratedSystems = viewModel.IntegratedSystems,

        //        // الدعم التقني / التنظيمي
        //        // SupportingDocs = request.SupportingDocs
        //        DurationWithUnit = viewModel.DurationWithUnit,
        //        UserId = viewModel.UserId,
        //        SectionNotes = viewModel.SectionNotes,
        //        DepartmentNotes = viewModel.DepartmentNotes,
        //    };

        //    _context.Request.Add(request);
        //    _context.SaveChanges();
        //    return request;
        //}

        //public async Task<FormData> CreateForManagerAsync(RequestViewModel viewModel, User currentUser)
        //{
        //    if (!await _userManager.IsInRoleAsync(currentUser, "Manager"))
        //        throw new UnauthorizedAccessException("Only managers can submit this request.");

        //    var request = new FormData
        //    {
        //        RequestId = viewModel.RequestId,
        //        ServiceName = viewModel.ServiceName,
        //        ServiceType = viewModel.ServiceType,
        //        ServiceDescription = viewModel.ServiceDescription,
        //        HasDependencies = viewModel.HasDependencies,
        //        DependencyExplanation = viewModel.DependencyExplanation,
        //        ProcedureNumber = viewModel.ProcedureNumber,
        //        ProcedureFileUrl = viewModel.ProcedureFileUrl,

        //        AllowedSubmissionCount = viewModel.AllowedSubmissionCount,
        //        ServiceFee = viewModel.ServiceFee,
        //        TargetAudience = viewModel.TargetAudience,
        //        ResponsibleDepartment = viewModel.ResponsibleDepartment,
        //        AvailableCities = viewModel.AvailableCities,

        //        ApproverDepartment = viewModel.ApproverDepartment,
        //        ApproverName = viewModel.ApproverName,
        //        ActionType = viewModel.ActionType,

        //        FinalOutputDescription = viewModel.FinalOutputDescription,

        //        FieldName = viewModel.FieldName,
        //        FieldType = viewModel.FieldType,
        //        FieldNotes = viewModel.FieldNotes,
        //        IsFieldRequired = viewModel.IsFieldRequired,
        //        FieldEnglishName = viewModel.FieldEnglishName,

        //        FieldCategories = viewModel.FieldCategories,
        //        FieldRelationships = viewModel.FieldRelationships,

        //        ServiceConditions = viewModel.ServiceConditions,

        //        ExecutionPath = viewModel.ExecutionPath,
        //        ExecutionDuration = viewModel.ExecutionDuration,

        //        IntegratedSystems = viewModel.IntegratedSystems,
        //        DurationWithUnit = viewModel.DurationWithUnit,
        //        UserId = viewModel.UserId,
        //        SectionNotes = viewModel.SectionNotes,
        //        DepartmentNotes = viewModel.DepartmentNotes
        //    };

        //    _context.Request.Add(request);
        //    await _context.SaveChangesAsync();

        //    return request;
        //}



        public void Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FormData>> GetAllWithUserAsync()
        {
            return await _context.Forms
                .Include(r => r.User)
                .ThenInclude(u => u.Department) // Optional: if you need department info
                .ToListAsync();
        }


        public FormData GetById(int Id)
        {
            var reqInDb = _context.Forms.Find(Id);
            return reqInDb;
        }
        public async Task<FormData> GetByIdAsync(int requestId)
        {
            return await _context.Forms
                .Include(r => r.User) // Include related data if needed
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(FormData request)
        {
            var existing = _context.Forms.Where(x=> x.RequestId == request.RequestId).FirstOrDefault();
            if (existing == null) throw new InvalidOperationException("Request not found");

            // Update only needed fields (optional but safer)  
            existing.SectionNotes = request.SectionNotes;
            existing.DepartmentNotes = request.DepartmentNotes;
            // ... other fields  

            _context.SaveChanges();
        }
        //
        public async Task<List<FormData>> GetRequestsForManagerDepartmentAsync(string managerId,int userSectionId)
        {
            var managerRequests = await _context.Forms.Include(r => r.User)
                .Include(f => f.Attachments)
                .ThenInclude(a => a.AttachmentData)
                .Include(f => f.Descriptions)
                .Include(a => a.RequestActions)
                .ThenInclude(u => u.Department)
                .Where(r => r.RequestActions.SectionId == userSectionId &&  r.RequestActions.LevelId == 1)
                .ToListAsync();

            return managerRequests;
           
        }


        public async Task<List<FormData>> GetRequestsFromManagersInDepartmentAsync(string bigManagerId, int userDepartmentId)
        {

            var managerRequests = await _context.Forms.Include(r => r.User)

               .Include(a => a.RequestActions)
               .ThenInclude(u => u.Department)
               .Where(r => r.RequestActions.DepartmentId == userDepartmentId && r.RequestActions.LevelId == 2)
               .ToListAsync();

            return managerRequests;
        //    var bigManager = await _context.Users
        //.Include(u => u.Department)
        //.FirstOrDefaultAsync(u => u.Id == bigManagerId);

        //    if (bigManager == null || bigManager.DepartmentId == null)
        //        return new List<Request>();

        //    var departmentId = bigManager.DepartmentId;

        //    // Get request IDs with StatusId == 2 and department match
        //    var requestIds = await _context.RequestActions
        //        .Where(ra => ra.StatusId == 2 && ra.DepartmentId == departmentId)
        //        .Select(ra => ra.RequestId)
        //        .ToListAsync();

        //    return await _context.Request
        //        .Include(r => r.User)
        //            .ThenInclude(u => u.Department)
        //        .Where(r => requestIds.Contains(r.RequestId))
        //        .ToListAsync();
        }

        public async Task<List<FormData>> GetRequestsForITManager(string ITManagerId, int userDepartmentId)
        {
            var managerRequests = await _context.Forms.Include(r => r.User)
                .Include(a => a.RequestActions)
                .ThenInclude(u => u.Department)
                .Where(r => r.RequestActions.LevelId == 3 || r.RequestActions.LevelId == 7)
                .ToListAsync();

            return managerRequests;

        }
        public async Task<List<FormData>> GetRequestsForApplicationManager(string ApplicationManagerId, int userDepartmentId)
        {
            var managerRequests = await _context.Forms.Include(r => r.User)
                .Include(a => a.RequestActions)
                .ThenInclude(u => u.Department)
                .Where(r => r.RequestActions.LevelId == 4 || r.RequestActions.LevelId == 6)
                .ToListAsync();

            return managerRequests;

        }

        public async Task<List<FormData>> GetRequestsForAnalyzer(string AnalyzerId, int userDepartmentId)
        {
            var caseStudyRequestIds = await _context.CaseStudy
                .Where(cs => cs.UserId == AnalyzerId)
                .Select(cs => cs.RequestId)
                .ToListAsync();

            var requests = await _context.Forms
                .Include(r => r.User)
                .Include(r => r.RequestActions)
                .ThenInclude(u => u.Department)
                .Where(r => caseStudyRequestIds.Contains(r.RequestId) && r.RequestActions.LevelId == 5)
                .ToListAsync();

            return requests;
        }

        public async Task<List<FormData>> GetRequestsByUserId(string UserId)
        {
            var requests = await _context.Forms.Include(r => r.User)
                .Include(r => r.User)
        .Include(r => r.RequestActions)
        .Where(r => r.UserId == UserId)
        .ToListAsync();

            return requests;

        }

       
        public async Task<bool> SendEmailAsync(RequestViewModel request, User currentUser)
        {
            try
            {
                var receiverEmail = currentUser.Email;
                var subject = "New Request Submitted";
                var message = $"A new request has been submitted by {currentUser.FirstName}. Please review it.";
                var mail = "abdoDX234@gmail.com";
                var pw = "ssfe txns mcyk rrms\r\n";
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(mail, pw);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage(mail, receiverEmail, subject, message);
                    await client.SendMailAsync(mailMessage);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }

        public Task<List<FormData>> GetRequestsForCaseStudyUsers(string CaseStudyId, int userDepartmentId)
        {
            throw new NotImplementedException();
        }

        public FormData Create(RequestViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public Task<FormData> CreateForSectionManagerAsync(RequestViewModel viewModel, User currentUser)
        {
            throw new NotImplementedException();
        }
    }
}

using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DSAR.Repositories
{
    public class CaseStudyRepository : ICaseStudyRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager; // Use User instead of IdentityUser

        public CaseStudyRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public void CreateCaseStudy(string analyzerUserId, int requestId)
        {
            if (string.IsNullOrEmpty(analyzerUserId))
                throw new ArgumentException("Analyzer user ID cannot be null or empty.");

            var request = _context.Forms.Find(requestId);
            if (request == null)
                throw new InvalidOperationException("Invalid RequestId: request not found.");

            var caseStudy = new CaseStudy
            {
                UserId = analyzerUserId,
                RequestId = requestId
            };

            _context.CaseStudy.Add(caseStudy);
            _context.SaveChanges();
        }

        public async Task<List<CaseStudyViewModel>> GetCaseStudiesByUserIdAsync(string userId)
        {
            var caseStudies = await _context.CaseStudy
                .Include(cs => cs.User)
                .Include(cs => cs.Request)
                .Where(cs => cs.UserId == userId)
                .Select(cs => new CaseStudyViewModel
                {
                    CaseId = cs.CaseId,
                    RequestId = cs.RequestId,
                    UserId = cs.UserId,
                    FirstName = cs.User.FirstName,
                    LastName = cs.User.LastName,
                })
                .ToListAsync();

            return caseStudies;
        }
        public async Task<CaseStudy?> GetCaseStudyByUserIdAsync(string userId, int requestId)
        {
            return await _context.CaseStudy
       .Where(cs => cs.UserId == userId && cs.RequestId == requestId)
       .OrderByDescending(cs => cs.CreatedAt) // Make sure CreatedAt is a DateTime
       .FirstOrDefaultAsync();
        }
        public async Task<CaseStudy?> GetByIdAsync(int caseId)
        {
            return await _context.CaseStudy.FindAsync(caseId);
        }
        public async Task<CaseStudy?> GetByRequestIdAsync(int requestId)
        {
            return await _context.CaseStudy
                .Include(cs => cs.User)       // Optional: only if you need user details
                .Include(cs => cs.Request)    // Optional: only if you need request details
                .FirstOrDefaultAsync(cs => cs.RequestId == requestId);
        }

        public void Update(CaseStudy caseStudy)
        {
            _context.CaseStudy.Update(caseStudy);
            _context.SaveChanges();
        }
        public async Task AddAttachmentAsync(int caseStudyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            var metadata = new CaseStudyAttachmentMetadata
            {
                CaseStudyId = caseStudyId,
                FileName = Path.GetFileNameWithoutExtension(file.FileName), // <-- fix here
                FileExtension = Path.GetExtension(file.FileName),
                FileSize = file.Length,
                FieldName = "CaseStudyAttachment"
            };

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var data = new CaseStudyAttachmentData
            {
                Data = ms.ToArray()
            };

            // Add metadata first so EF can generate the PK
            await _context.CaseStudyAttachmentMetadata.AddAsync(metadata);
            await _context.SaveChangesAsync();

            // Set PK for data to match metadata Id
            data.Id = metadata.Id;

            await _context.CaseStudyAttachmentData.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        // Retrieve attachments for a case study (optional)
        public async Task<List<CaseStudyAttachmentMetadata>> GetAttachmentsByCaseStudyIdAsync(int caseStudyId)
        {
            return await _context.CaseStudyAttachmentMetadata
                .Include(m => m.CaseStudyAttachmentData)
                .Where(m => m.CaseStudyId == caseStudyId)
                .ToListAsync();
        }
        public async Task<CaseStudyAttachmentData?> GetAttachmentDataByIdAsync(int id)
        {
            return await _context.CaseStudyAttachmentData
                .Include(d => d.CaseStudyAttachmentMetadata)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

    }

}

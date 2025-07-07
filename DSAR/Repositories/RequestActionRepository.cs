using Azure.Core;
using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DSAR.Repositories
{
    public class RequestActionRepository : IRequestActionRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public RequestActionRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public RequestActions Create(RequestViewModel viewModel, User currentUser, FormData request)
        {


            var requestActions = new RequestActions
            {
                RequestId = request.RequestId,
                UserId = currentUser.Id,
                DepartmentId = currentUser.DepartmentId ?? 0,
                SectionId = currentUser.SectionId ?? 0,
                StatusId = 1,
                LevelId = 1
            };

            _context.RequestActions.Add(requestActions);
            _context.SaveChanges();
            return requestActions;
        }
        public RequestActions CreateSectionManager(RequestViewModel viewModel, User currentUser, FormData request)
        {


            var requestActions = new RequestActions
            {
                RequestId = request.RequestId,
                UserId = currentUser.Id,
                DepartmentId = currentUser.DepartmentId ?? 0,
                SectionId = currentUser.SectionId ?? 0,
                StatusId = 1,
                LevelId = 2
            };

            _context.RequestActions.Add(requestActions);
            _context.SaveChanges();
            return requestActions;
        }

        public RequestActions CreateDepartmentManager(RequestViewModel viewModel, User currentUser, FormData request)
        {


            var requestActions = new RequestActions
            {
                RequestId = request.RequestId,
                UserId = currentUser.Id,
                DepartmentId = currentUser.DepartmentId ?? 0,
                SectionId = currentUser.SectionId ?? 0,
                StatusId = 1,
                LevelId = 3
            };

            _context.RequestActions.Add(requestActions);
            _context.SaveChanges();
            return requestActions;
        }

        public RequestActions CreateITManager(RequestViewModel viewModel, User currentUser, FormData request)
        {


            var requestActions = new RequestActions
            {
                RequestId = request.RequestId,
                UserId = currentUser.Id,
                DepartmentId = currentUser.DepartmentId ?? 0,
                SectionId = currentUser.SectionId ?? 0,
                StatusId = 1,
                LevelId = 4
            };

            _context.RequestActions.Add(requestActions);
            _context.SaveChanges();
            return requestActions;
        }


        public void Create(RequestActions request)
        {
            throw new NotImplementedException();
        }

        public void Delete(string Id)
        {
            var req = _context.RequestActions.Find(Id);
            if (req == null)
                throw new InvalidOperationException("User not found.");

            _context.RequestActions.Remove(req);
            _context.SaveChanges();
        }

        public IEnumerable<RequestActions> GetAll()
        {
            return _context.RequestActions
         .Include(r => r.Status)
         .Include(r => r.Department)
         .Include(r => r.User) // assuming RequestActions.User navigation exists
         .ToList();
        }
        public IEnumerable<RequestActions> GetAllByUserId(string userId)
        {
            return _context.RequestActions
                .Include(r => r.User)
                .Include(r => r.Status)
                .Include(r => r.Department)
                .Include(r => r.FormData)
                .Where(r => r.UserId == userId)
                .ToList();
        }
        public RequestActions GetById(string Id)
        {
            var req = _context.RequestActions.Find(Id);
            return req;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(RequestActions request)
        {
            // Fix for CS0136 and CS0841: Rename the variable to avoid conflict with the parameter name.
            var existingRequest = _context.RequestActions.Find(request.ActionId);
            if (existingRequest == null)
            {
                throw new InvalidOperationException("Request not found.");
            }

            // Update properties of the existing request.
            existingRequest.StatusId = request.StatusId;
            existingRequest.LevelId = request.LevelId;
            existingRequest.DepartmentId = request.DepartmentId;
            existingRequest.UserId = request.UserId;

            // Save changes.
            _context.RequestActions.Update(existingRequest);
            _context.SaveChanges();
        }
        public async Task<RequestActions> GetRequestActionByRequestIdAsync(int requestId)
        {
            return await _context.RequestActions
                       .FirstOrDefaultAsync(ra => ra.RequestId == requestId);

        }
        public async Task<RequestActions?> GetByIdAsync(int id)
        {
            return await _context.RequestActions.FindAsync(id);
        }

        public async Task<List<RequestActions>> GetCompeleteRequestsByUserId(string UserId)
        {
            return await _context.RequestActions
                .Include(r => r.User)
                .Include(r => r.Status)
                .Include(r => r.Department)
                .Include(r => r.FormData)
                .Where(r => r.UserId == UserId && (r.LevelId == 8 || r.LevelId == 9)) // Assuming 2 is the ID for completed status
                .ToListAsync();

        }

        public async Task<List<RequestActions>> GetRequestsStillInProcessByUserId(string UserId)
        {
            return await _context.RequestActions
                .Include(r => r.User)
                .Include(r => r.Status)
                .Include(r => r.Department)
                .Include(r => r.FormData)
                .Where(r => r.UserId == UserId && r.LevelId != 8 && r.LevelId != 9)
                .ToListAsync();

        }
    }
}

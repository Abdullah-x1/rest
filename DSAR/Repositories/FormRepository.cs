using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.Repository;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DSAR.Repository
{
    public class FormRepository : iFormRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestRepository _requestRepository;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public FormRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor,IRequestRepository requestRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _requestRepository = requestRepository;
        }

        #region Main Form Operations
        public async Task<IEnumerable<FormData>> GetAll()
        {
            return await _context.Forms
                .Include(f => f.Attachments)
                .ThenInclude(a => a.AttachmentData)
                .Include(f => f.Descriptions)
                .ToListAsync();
        }
        public async Task<AttachmentData> GetAttachmentById(int attachmentId)
        {
            return await _context.AttachmentData
                .Include(a => a.AttachmentMetadata)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }

        public async Task<List<AttachmentMetadata>> GetAttachmentsForForm(int formId)
        {
            return await _context.AttachmentMetadata
                .Where(a => a.FormDataId == formId)
                .ToListAsync();
        }
        public async Task<FormData> GetById(int id)
        {
            return await _context.Forms
                .Include(f => f.Attachments)
                .ThenInclude(a => a.AttachmentData)
                .Include(f => f.Descriptions)
                .FirstOrDefaultAsync(f => f.RequestId == id);
        }

        public async Task<List<DescriptionEntry>> GetDescriptionsByRequestId(int requestId)
        {
            return await _context.DescriptionEntries
                .Where(d => d.RequestId == requestId)
                .ToListAsync();
        }


        public async Task Create(FormData data)
        {
            await _context.Forms.AddAsync(data);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CreateWithAttachment(FormData data)
        {
            await _context.Forms.AddAsync(data);
        }
        #endregion

        #region Snapshot Management
        private string GetOrCreateCookieId()
        {
            var context = _httpContextAccessor.HttpContext;
            const string cookieName = "SnapshotCookieId";

            if (context.Request.Cookies.TryGetValue(cookieName, out var cookieId) && !string.IsNullOrWhiteSpace(cookieId))
                return cookieId;

            cookieId = Guid.NewGuid().ToString();

            context.Response.Cookies.Append(cookieName, cookieId, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = context.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return cookieId;
        }


        private async Task<SnapshotFormData> GetOrCreateSnapshotAsync()
        {
            var cookieId = GetOrCreateCookieId();
            var snapshot = await _context.SnapshotForms
                .Include(s => s.Attachments)
                .ThenInclude(a => a.SnapshotAttachmentData)
                .Include(s => s.Descriptions)
                .FirstOrDefaultAsync(s => s.CookieId == cookieId);

            if (snapshot == null)
            {
                snapshot = new SnapshotFormData
                {
                    CookieId = cookieId,

                    FormDataJson = JsonSerializer.Serialize(new FormData(), _jsonOptions)
                };
                await _context.SnapshotForms.AddAsync(snapshot);
                await _context.SaveChangesAsync();
            }

            return snapshot;
        }

        public async Task ClearCurrentSnapshot()
        {
            var cookieId = GetOrCreateCookieId();
            var snapshot = await _context.SnapshotForms
                .FirstOrDefaultAsync(s => s.CookieId == cookieId);


            if (snapshot != null)
            {
                _context.SnapshotForms.Remove(snapshot);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RequestViewModel> GetCurrentFormData()
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var formData = snapshot.GetFormData(_jsonOptions);

            // Map FormData to RequestViewModel  
            var requestViewModel = new RequestViewModel
            {
                Name = formData.Name,
                Email = formData.Email,
                Message = formData.Message,
                Field1 = formData.Field1,
                Field2 = formData.Field2,
                Field3 = formData.Field3,
                Depend = formData.Depend,
                Field4 = formData.Field4,
                Field5 = formData.Field5,
                Field6 = formData.Field6,
                RepeatLimit = formData.RepeatLimit,
                Fees = formData.Fees,
                Cities = formData.Cities,
                TargetAudience = formData.TargetAudience,
                
                ExpectedOutput1 = formData.ExpectedOutput1,
                ExpectedOutput2 = formData.ExpectedOutput2,
                ApprovedTemplate = formData.ApprovedTemplate,
                DetailedInfo = formData.DetailedInfo,
                RequiredConditions = formData.RequiredConditions,
                Workflow = formData.Workflow,
                UploadsRequired = formData.UploadsRequired,
                Documents = formData.Documents,
                Timeline = formData.Timeline,
                SystemNeeded = formData.SystemNeeded,
                Cities2 = formData.Cities2,
                DepartmentHeadName = formData.DepartmentHeadName,
                AdditionalNotes = formData.AdditionalNotes,
                Departments = _requestRepository.GetAllDepartments()
    .Select(d => new SelectListItem
    {
        Value = d.DepartmentId.ToString(),       // or d.DepartmentId, depending on your model
        Text = d.DepartmentName             // or d.DepartmentName or similar
    }).ToList()

            };
            requestViewModel.Attachment1Id = snapshot.Attachments
        .Where(a => a.FieldName == "Step1")
        .Select(a => a.Id)
        .FirstOrDefault();

            requestViewModel.Attachment1Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step1")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            requestViewModel.Attachment2Id = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_1")
                .Select(a => a.Id)
                .FirstOrDefault();

            requestViewModel.Attachment2Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_1")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            requestViewModel.Attachment3Id = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_2")
                .Select(a => a.Id)
                .FirstOrDefault();

            requestViewModel.Attachment3Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_2")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            requestViewModel.WorkflowAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_Workflow")
                .Select(a => a.Id)
                .FirstOrDefault();

            requestViewModel.WorkflowName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_Workflow")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            requestViewModel.UploadsRequiredAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_uploadsRequiredFile")
                .Select(a => a.Id)
                .FirstOrDefault();

            requestViewModel.UploadsRequiredName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_uploadsRequiredFile")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            requestViewModel.DocumentsAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_documentsFile")
                .Select(a => a.Id)
                .FirstOrDefault();

            requestViewModel.DocumentsName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_documentsFile")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .FirstOrDefault();

            return requestViewModel;
        }
        #endregion

        #region Step Handlers
        public async Task<bool> HandleStep1Data(RequestViewModel data, IFormFile attachment)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);

            // Check if there are meaningful changes or a new attachment
            if (!HasMeaningfulChanges(currentData, data) && attachment == null)
            {
                // No changes detected, return false to indicate nothing saved
                return false;
            }

            // Update the data with new values
            currentData.Field1 = data.Field1;
            currentData.Field2 = data.Field2;
            currentData.Field3 = data.Field3;
            currentData.Depend = data.Depend;
            currentData.Field4 = data.Field4;
            currentData.Field5 = data.Field5;
            currentData.Field6 = data.Field6;

            snapshot.SetFormData(currentData, _jsonOptions);

            // If an attachment was uploaded, handle it
            if (attachment != null && attachment.Length > 0)
            {
                await HandleAttachment(attachment, snapshot, "Step1");
            }

            // Save changes to database
            await _context.SaveChangesAsync();

            return true;
        }

        private bool HasMeaningfulChanges(FormData current, RequestViewModel incoming)
        {
            return current.Field1 != incoming.Field1 ||
                   current.Field2 != incoming.Field2 ||
                   current.Field3 != incoming.Field3 ||
                   current.Depend != incoming.Depend ||
                   current.Field4 != incoming.Field4 ||
                   current.Field5 != incoming.Field5 ||
                   current.Field6 != incoming.Field6;
        }

        public async Task<bool> HandleStep2Data(RequestViewModel data, IFormFile attachment1, IFormFile attachment2)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);
            if (!HasMeaningfulChangesStep2(currentData, data) && attachment1 == null && attachment2 == null)
                return false;
            // Update only step 2 fields
            currentData.Name = data.Name;
            currentData.Email = data.Email;
            currentData.RepeatLimit = data.RepeatLimit;
            currentData.Fees = data.Fees;
            currentData.Cities = data.Cities;
            currentData.TargetAudience = data.TargetAudience;
            currentData.DepartmentId = data.DepartmentId;
            currentData.ExpectedOutput1 = data.ExpectedOutput1;
            currentData.ExpectedOutput2 = data.ExpectedOutput2;
            currentData.ApprovedTemplate = data.ApprovedTemplate;
            currentData.DetailedInfo = data.DetailedInfo;
            currentData.RequiredConditions = data.RequiredConditions;

            snapshot.SetFormData(currentData, _jsonOptions);

            if (attachment1 != null && attachment1.Length > 0)
            {
                await HandleAttachment(attachment1, snapshot, "Step2_1");
            }

            if (attachment2 != null && attachment2.Length > 0)
            {
                await HandleAttachment(attachment2, snapshot, "Step2_2");
            }
            //var request = new FormData
            //{
            //   Name = data.Name,
            //    Email = data.Email,
            //    RepeatLimit = data.RepeatLimit,
            //    Fees = data.Fees,
            //    Cities = data.Cities,
            //    TargetAudience = data.TargetAudience,
            //    DepName = data.DepName,
            //    ExpectedOutput1 = data.ExpectedOutput1,
            //    ExpectedOutput2 = data.ExpectedOutput2,
            //    ApprovedTemplate = data.ApprovedTemplate,
            //    DetailedInfo = data.DetailedInfo,
            //    RequiredConditions = data.RequiredConditions,

            //};

            await _context.SaveChangesAsync();
            return true;
        }

        private bool HasMeaningfulChangesStep2(FormData current, RequestViewModel incoming)
        {
            return current.Name != incoming.Name ||
                   current.Email != incoming.Email ||
                   current.RepeatLimit != incoming.RepeatLimit ||
                   current.Fees != incoming.Fees ||
                   current.Cities != incoming.Cities ||
                   current.TargetAudience != incoming.TargetAudience ||
                   current.DepartmentId != incoming.DepartmentId ||
                   current.ExpectedOutput1 != incoming.ExpectedOutput1 ||
                   current.ExpectedOutput2 != incoming.ExpectedOutput2 ||
                   current.ApprovedTemplate != incoming.ApprovedTemplate ||
                   current.DetailedInfo != incoming.DetailedInfo ||
                   current.RequiredConditions != incoming.RequiredConditions;
        }


        public async Task<FormData> HandleStep3Data(RequestViewModel data, string UserId)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var snapshotData = snapshot.GetFormData(_jsonOptions);
            

            // Map all properties from JSON to the final form data
            data.Name = snapshotData.Name;
            data.Email = snapshotData.Email;
            data.Message = snapshotData.Message;
            data.Field1 = snapshotData.Field1;
            data.Field2 = snapshotData.Field2;
            data.Field3 = snapshotData.Field3;
            data.Depend = snapshotData.Depend;
            data.Field4 = snapshotData.Field4;
            data.Field5 = snapshotData.Field5;
            data.Field6 = snapshotData.Field6;
            data.RepeatLimit = snapshotData.RepeatLimit;
            data.Fees = snapshotData.Fees;
            data.Cities = snapshotData.Cities;
            data.TargetAudience = snapshotData.TargetAudience;
            data.DepartmentId = snapshotData.DepartmentId;
            data.ExpectedOutput1 = snapshotData.ExpectedOutput1;
            data.ExpectedOutput2 = snapshotData.ExpectedOutput2;
            data.ApprovedTemplate = snapshotData.ApprovedTemplate;
            data.DetailedInfo = snapshotData.DetailedInfo;
            data.RequiredConditions = snapshotData.RequiredConditions;
            data.Workflow = snapshotData.Workflow;
            data.UploadsRequired = snapshotData.UploadsRequired;
            data.Documents = snapshotData.Documents;
            data.Timeline = snapshotData.Timeline;
            data.SystemNeeded = snapshotData.SystemNeeded;
            data.Cities2 = snapshotData.Cities2;
            data.DepartmentHeadName = snapshotData.DepartmentHeadName;
            data.AdditionalNotes = snapshotData.AdditionalNotes;
            

            // Handle attachments
            foreach (var attachment in snapshot.Attachments)
            {
                data.Attachments.Add(new AttachmentMetadata
                {
                    FileName = attachment.FileName,
                    FileExtension = attachment.FileExtension,
                    FileSize = attachment.FileSize,

                    FieldName = attachment.FieldName, // Copy field name

                    AttachmentData = new AttachmentData
                    {
                        Data = attachment.SnapshotAttachmentData.Data
                    }
                });
            }

            var request = new FormData
            {
                UserId = data.UserId,
                Name = data.Name,
                Email = data.Email,
                Message = data.Message,
                Field1 = data.Field1,
                Field2 = data.Field2,
                Field3 = data.Field3,
                Depend =    data.Depend,
                Field4 = data.Field4,
                Field5 = data.Field5,
                Field6 = data.Field6,
                RepeatLimit = data.RepeatLimit,
                Fees = data.Fees,
                Cities = data.Cities,
                TargetAudience = data.TargetAudience,
                DepartmentId = data.DepartmentId,
                ExpectedOutput1 = data.ExpectedOutput1,
                ExpectedOutput2 = data.ExpectedOutput2,
                ApprovedTemplate = data.ApprovedTemplate,
                DetailedInfo = data.DetailedInfo,
                RequiredConditions = data.RequiredConditions,
                Workflow = data.Workflow,
                UploadsRequired = data.UploadsRequired,
                Documents = data.Documents,
                Timeline = data.Timeline,
                SystemNeeded = data.SystemNeeded,
                Cities2 = data.Cities2,
                DepartmentHeadName = data.DepartmentHeadName,
                AdditionalNotes = data.AdditionalNotes,
                FilePath = data.FilePath,
                SectionNotes = data.SectionNotes,
                DepartmentNotes = data.DepartmentNotes,
                Attachments = data.Attachments,
                
            };
            // Fix for CS0029: Cannot implicitly convert type 'string' to 'int'
            // The issue is that `RequestNumber` is defined as an `int` in the `FormData` class, but the code is attempting to assign a string value to it.
            // To resolve this, we need to either change the type of `RequestNumber` to `string` in the `FormData` class or modify the assignment to ensure it matches the `int` type.
            int userRequestCount = await _context.Forms
                .Include(r => r.User)
            .Where(r => r.User.UserId == UserId)
               .CountAsync();
            string newRequestNumber = $"{UserId}{(userRequestCount + 1).ToString("D4")}";
            request.RequestNumber = Double.Parse(newRequestNumber);


            // Create the actual form data
            await _context.Forms.AddAsync(request);
            await _context.SaveChangesAsync();

            // Save descriptions
            foreach (var description in snapshot.Descriptions)
            {
                _context.DescriptionEntries.Add(new DescriptionEntry
                {
                    RequestId = request.RequestId,
                    Description1 = description.Description1,
                    Description2 = description.Description2
                });
            }

            await _context.SaveChangesAsync();

            // Clean up the snapshot
            _context.SnapshotForms.Remove(snapshot);
            await _context.SaveChangesAsync();

            return request;
        }



        public async Task<(bool isSaved, string workflowName, string uploadsName, string documentsName)> HandleStep4Data(
        RequestViewModel data,
        IFormFile workflowFile,
        IFormFile uploadsRequiredFile,
        IFormFile documentsFile)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);
            if (!HasMeaningfulChangesStep4(currentData, data) &&
    (workflowFile == null || workflowFile.Length == 0) &&
    (uploadsRequiredFile == null || uploadsRequiredFile.Length == 0) &&
    (documentsFile == null || documentsFile.Length == 0))
            {
                // No changes detected, skip saving
                return (false, null, null, null);
            }
            // Check if anything changed
            bool hasChanges =
                currentData.Workflow != data.Workflow ||
                currentData.UploadsRequired != data.UploadsRequired ||
                currentData.Documents != data.Documents ||
                currentData.Timeline != data.Timeline ||
                currentData.SystemNeeded != data.SystemNeeded ||
                currentData.Cities2 != data.Cities2 ||
                currentData.DepartmentHeadName != data.DepartmentHeadName ||
                currentData.AdditionalNotes != data.AdditionalNotes ||
                (workflowFile != null && workflowFile.Length > 0) ||
                (uploadsRequiredFile != null && uploadsRequiredFile.Length > 0) ||
                (documentsFile != null && documentsFile.Length > 0);

            if (!hasChanges)
                return (false, null, null, null);

            // Update fields
            currentData.Workflow = data.Workflow;
            currentData.UploadsRequired = data.UploadsRequired;
            currentData.Documents = data.Documents;
            currentData.Timeline = data.Timeline;
            currentData.SystemNeeded = data.SystemNeeded;
            currentData.Cities2 = data.Cities2;
            currentData.DepartmentHeadName = data.DepartmentHeadName;
            currentData.AdditionalNotes = data.AdditionalNotes;

            snapshot.SetFormData(currentData, _jsonOptions);

            string workflowName = null, uploadsName = null, documentsName = null;

            if (workflowFile != null && workflowFile.Length > 0)
            {
                await HandleAttachment(workflowFile, snapshot, "Step4_Workflow");
                workflowName = Path.GetFileName(workflowFile.FileName);
            }

            if (uploadsRequiredFile != null && uploadsRequiredFile.Length > 0)
            {
                await HandleAttachment(uploadsRequiredFile, snapshot, "Step4_uploadsRequiredFile");
                uploadsName = Path.GetFileName(uploadsRequiredFile.FileName);
            }

            if (documentsFile != null && documentsFile.Length > 0)
            {
                await HandleAttachment(documentsFile, snapshot, "Step4_documentsFile");
                documentsName = Path.GetFileName(documentsFile.FileName);
            }

            await _context.SaveChangesAsync();
            return (true, workflowName, uploadsName, documentsName);
        }
        private bool HasMeaningfulChangesStep4(FormData current, RequestViewModel incoming)
        {
            return current.Workflow != incoming.Workflow ||
                   current.UploadsRequired != incoming.UploadsRequired ||
                   current.Documents != incoming.Documents ||
                   current.Timeline != incoming.Timeline ||
                   current.SystemNeeded != incoming.SystemNeeded ||
                   current.Cities2 != incoming.Cities2 ||
                   current.DepartmentHeadName != incoming.DepartmentHeadName ||
                   current.AdditionalNotes != incoming.AdditionalNotes;
        }

        #endregion

        #region Description Handling
        public async Task<bool> HandleDescriptions(List<DescriptionEntry> descriptions)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var existingDescriptions = snapshot.Descriptions
                .Select(d => new DescriptionEntry
                {
                    Description1 = d.Description1,
                    Description2 = d.Description2
                })
                .ToList();

            if (!HasMeaningfulChangesDescriptions(existingDescriptions, descriptions))
                return false;

            // Clear existing and add new
            _context.SnapshotDescriptionEntries.RemoveRange(snapshot.Descriptions);
            await _context.SaveChangesAsync();

            foreach (var desc in descriptions)
            {
                snapshot.Descriptions.Add(new SnapshotDescriptionEntry
                {
                    Description1 = desc.Description1,
                    Description2 = desc.Description2
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }
        private bool HasMeaningfulChangesDescriptions(List<DescriptionEntry> current, List<DescriptionEntry> incoming)
        {
            if (current.Count != incoming.Count)
                return true;

            for (int i = 0; i < current.Count; i++)
            {
                if (current[i].Description1?.Trim() != incoming[i].Description1?.Trim() ||
                    current[i].Description2?.Trim() != incoming[i].Description2?.Trim())
                {
                    return true;
                }
            }

            return false;
        }


        public async Task<List<DescriptionEntry>> GetDescriptions()
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            return snapshot.Descriptions.Select(d => new DescriptionEntry
            {
                Description1 = d.Description1,
                Description2 = d.Description2
            }).ToList();
        }
        #endregion

        #region Helper Methods
        private async Task HandleAttachment(IFormFile attachment, SnapshotFormData snapshot, string fieldName)
        {
            if (attachment == null || attachment.Length == 0) return;

            // Get file extension
            var extension = Path.GetExtension(attachment.FileName).ToLower();

            // Define allowed extensions
            var allowedExtensions = new[]
            {
        ".doc", ".docx", ".xls", ".xlsx", ".pdf", // Office files
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" // Image files
    };

            // Validate file type
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Unsupported file type: {extension}");

        

            // Remove any existing attachments for this field
            var existing = snapshot.Attachments
                .Where(a => a.FieldName == fieldName)
                .ToList();

            foreach (var att in existing)
            {
                snapshot.Attachments.Remove(att);
            }

            // Save new attachment
            var attachmentMetadata = new SnapshotAttachmentMetadata
            {
                FileName = Path.GetFileNameWithoutExtension(attachment.FileName),
                FileExtension = extension,
                FieldName = fieldName,
                FileSize = attachment.Length,
                SnapshotAttachmentData = new SnapshotAttachmentData
                {
                    Data = await GetFileBytesAsync(attachment)
                }
            };

            snapshot.Attachments.Add(attachmentMetadata);
        }
        public async Task<SnapshotAttachmentData> GetSnapshotAttachmentById(int id)
        {
            return await _context.SnapshotAttachmentDatas
                .Include(d => d.SnapshotAttachmentMetadata)
                .FirstOrDefaultAsync(d => d.Id == id);
        }



        private async Task<byte[]> GetFileBytesAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
        #endregion
    }
}
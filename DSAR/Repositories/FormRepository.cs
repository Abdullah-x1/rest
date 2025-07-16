using Azure.Core;
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
using System.Security.Claims;
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
        private readonly ICityRepository _cityRepository;


        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public FormRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor, IRequestRepository requestRepository,ICityRepository cityRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _requestRepository = requestRepository;
            _cityRepository = cityRepository;
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

      
      

        private async Task<SnapshotFormData> GetOrCreateSnapshotAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var snapshot = await _context.SnapshotForms
                .Include(s => s.Attachments)
                .ThenInclude(a => a.SnapshotAttachmentData)
                .Include(s => s.Descriptions)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (snapshot == null)
            {
                snapshot = new SnapshotFormData
                {
                    UserId = userId,
                    FormDataJson = JsonSerializer.Serialize(new FormData(), _jsonOptions),
                            TermsAccepted = false
                };
                await _context.SnapshotForms.AddAsync(snapshot);
                await _context.SaveChangesAsync();
            }

            return snapshot;
        }

        public async Task ClearCurrentSnapshot()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var snapshot = await _context.SnapshotForms.FirstOrDefaultAsync(s => s.UserId == userId);
            if (snapshot != null)
            {
                _context.SnapshotForms.Remove(snapshot);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<RequestViewModel> GetCurrentFormData()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var snapshot = await GetOrCreateSnapshotAsync();

            var formData = snapshot.GetFormData(_jsonOptions);
            // Map FormData to RequestViewModel  
            var requestViewModel = new RequestViewModel
            {
                Name = formData.Name,
                Email = formData.Email,
                Message = formData.Message,
                ServiceName = formData.ServiceName,
                ServiceTypeAndLocation = formData.ServiceTypeAndLocation,
                ServiceDescription = formData.ServiceDescription,
                HasDependency = formData.HasDependency,
                DependencyDetails = formData.DependencyDetails,
                ProcedureNumber = formData.ProcedureNumber,
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
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                }).ToList(),
                //Cities = _cityRepository.GetAll()
                //.Select(d => new SelectListItem
                //{
                //    Value = d.CityId.ToString(),
                //    Text = d.CityName
                //}).ToList()

            };
            requestViewModel.Attachment1Id = snapshot.Attachments
         .Where(a => a.FieldName == "Step1")
         .Select(a => a.Id)
         .ToList();

            requestViewModel.Attachment1Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step1")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            requestViewModel.Attachment2Id = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_1")
                .Select(a => a.Id)
                .ToList();

            requestViewModel.Attachment2Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_1")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            requestViewModel.Attachment3Id = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_2")
                .Select(a => a.Id)
                .ToList();

            requestViewModel.Attachment3Name = snapshot.Attachments
                .Where(a => a.FieldName == "Step2_2")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            requestViewModel.WorkflowAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_Workflow")
                .Select(a => a.Id)
                .ToList();

            requestViewModel.WorkflowName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_Workflow")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            requestViewModel.UploadsRequiredAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_uploadsRequiredFile")
                .Select(a => a.Id)
                .ToList();

            requestViewModel.UploadsRequiredName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_uploadsRequiredFile")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            requestViewModel.DocumentsAttachmentId = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_documentsFile")
                .Select(a => a.Id)
                .ToList();

            requestViewModel.DocumentsName = snapshot.Attachments
                .Where(a => a.FieldName == "Step4_documentsFile")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();
            requestViewModel.CaseStudyAttachmentIds = snapshot.Attachments
    .Where(a => a.FieldName == "CaseStudyAttachment")
    .Select(a => a.Id)
    .ToList();

            requestViewModel.CaseStudyAttachmentNames = snapshot.Attachments
                .Where(a => a.FieldName == "CaseStudyAttachment")
                .Select(a => $"{a.FileName}{a.FileExtension}")
                .ToList();

            return requestViewModel;
        }
        #endregion

        #region Step Handlers
        public async Task<bool> HandleStep1Data(RequestViewModel data, List<IFormFile> attachments)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);

            // Check if there are meaningful changes or a new attachment
            if (!HasMeaningfulChanges(currentData, data) && (attachments == null || !attachments.Any()))
            {
                // No changes detected, return false to indicate nothing saved
                return false;
            }

            // Update the data with new values
            currentData.ServiceName = data.ServiceName;
            currentData.ServiceTypeAndLocation = data.ServiceTypeAndLocation;
            currentData.ServiceDescription = data.ServiceDescription;
            currentData.HasDependency = data.HasDependency;
            currentData.DependencyDetails = data.DependencyDetails;
            currentData.ProcedureNumber = data.ProcedureNumber;

            snapshot.SetFormData(currentData, _jsonOptions);

            // Clear existing attachments for "Step1"
            var existingAttachments = snapshot.Attachments
                .Where(a => a.FieldName == "Step1")
                .ToList();

            foreach (var existing in existingAttachments)
            {
                snapshot.Attachments.Remove(existing);
            }

            if (attachments != null && attachments.Any())
            {
                if (attachments.Count(f => f.Length > 0) > 5)
                    throw new InvalidOperationException("You can upload a maximum of 5 attachments for Step1.");

                foreach (var file in attachments.Where(f => f.Length > 0))
                {
                    await HandleAttachment(file, snapshot, "Step1");
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        private bool HasMeaningfulChanges(FormData current, RequestViewModel incoming)
        {
            return current.ServiceName != incoming.ServiceName ||
                   current.ServiceTypeAndLocation != incoming.ServiceTypeAndLocation ||
                   current.ServiceDescription != incoming.ServiceDescription ||
                   current.HasDependency != incoming.HasDependency ||
                   current.DependencyDetails != incoming.DependencyDetails ||
                   current.ProcedureNumber != incoming.ProcedureNumber;
        }

       

        public async Task<bool> HandleStep2Data(RequestViewModel data, List<IFormFile> attachments2, List<IFormFile> attachments3)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);

            bool hasFiles2 = attachments2 != null && attachments2.Any(f => f.Length > 0);
            bool hasFiles3 = attachments3 != null && attachments3.Any(f => f.Length > 0);

            if (!HasMeaningfulChangesStep2(currentData, data) && !hasFiles2 && !hasFiles3)
                return false;

            // Update Step 2 fields
            currentData.RepeatLimit = data.RepeatLimit;
            currentData.Fees = data.Fees;
            currentData.Cities2 = data.Cities2;
            currentData.TargetAudience = data.TargetAudience;
            currentData.DepartmentId = data.DepartmentId;
            currentData.ExpectedOutput1 = data.ExpectedOutput1;
            currentData.ExpectedOutput2 = data.ExpectedOutput2;
            currentData.ApprovedTemplate = data.ApprovedTemplate;
            currentData.DetailedInfo = data.DetailedInfo;
            currentData.RequiredConditions = data.RequiredConditions;

            snapshot.SetFormData(currentData, _jsonOptions);

            // Remove old attachments for Step2_1
            var toRemove2_1 = snapshot.Attachments.Where(a => a.FieldName == "Step2_1").ToList();
            foreach (var att in toRemove2_1)
                snapshot.Attachments.Remove(att);
            if (hasFiles2)
            {
                if (attachments2.Count(f => f.Length > 0) > 5)
                    throw new InvalidOperationException("You can upload a maximum of 5 attachments for Step2_1.");
                foreach (var file in attachments2.Where(f => f.Length > 0))
                    await HandleAttachment(file, snapshot, "Step2_1");
            }

            // Remove old attachments for Step2_2
            var toRemove2_2 = snapshot.Attachments.Where(a => a.FieldName == "Step2_2").ToList();
            foreach (var att in toRemove2_2)
                snapshot.Attachments.Remove(att);
            if (hasFiles3)
            {
                if (attachments3.Count(f => f.Length > 0) > 5)
                    throw new InvalidOperationException("You can upload a maximum of 5 attachments for Step2_2.");
                foreach (var file in attachments3.Where(f => f.Length > 0))
                    await HandleAttachment(file, snapshot, "Step2_2");
            }

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
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var snapshot = await GetOrCreateSnapshotAsync();
            var snapshotData = snapshot.GetFormData(_jsonOptions);

            MapSnapshotToViewModel(snapshotData, data);
            MapAttachments(snapshot, data);

            var request = await CreateFormDataFromViewModelAsync(data , UserId);
         

            await _context.Forms.AddAsync(request);
            await _context.SaveChangesAsync();

            SaveDescriptions(snapshot, request.RequestId);

            _context.SnapshotForms.Remove(snapshot);
            await _context.SaveChangesAsync();

            return request;
        }

        private void MapSnapshotToViewModel(FormData snapshotData, RequestViewModel data)
        {
            // Use reflection for a cleaner mapping or map manually if performance is key
            data.Name = snapshotData.Name;
            data.Email = snapshotData.Email;
            data.Message = snapshotData.Message;
            data.ServiceName = snapshotData.ServiceName;
            data.ServiceTypeAndLocation = snapshotData.ServiceTypeAndLocation;
            data.ServiceDescription = snapshotData.ServiceDescription;
            data.HasDependency = snapshotData.HasDependency;
            data.DependencyDetails = snapshotData.DependencyDetails;
            data.ProcedureNumber = snapshotData.ProcedureNumber;
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
        }
        private void MapAttachments(SnapshotFormData snapshot, RequestViewModel data)

        {
            foreach (var attachment in snapshot.Attachments)
            {
                data.Attachments.Add(new AttachmentMetadata
                {
                    FileName = attachment.FileName,
                    FileExtension = attachment.FileExtension,
                    FileSize = attachment.FileSize,
                    FieldName = attachment.FieldName,
                    AttachmentData = new AttachmentData
                    {
                        Data = attachment.SnapshotAttachmentData.Data
                    }
                });
            }
        }

        private async Task<FormData> CreateFormDataFromViewModelAsync(RequestViewModel data,string UserId)
        {
            var form =  new FormData
            {
                UserId = data.UserId,
                Name = data.Name,
                Email = data.Email,
                Message = data.Message,
                ServiceName = data.ServiceName,
                ServiceTypeAndLocation = data.ServiceTypeAndLocation,
                ServiceDescription = data.ServiceDescription,
                HasDependency = data.HasDependency,
                DependencyDetails = data.DependencyDetails,
                ProcedureNumber = data.ProcedureNumber,
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
              
                SectionNotes = data.SectionNotes,
                DepartmentNotes = data.DepartmentNotes,
                Attachments = data.Attachments,
            };
            int userRequestCount = await _context.Forms
                .Include(r => r.User)
            .Where(r => r.User.UserId == UserId)
               .CountAsync();
            string newRequestNumber = $"{UserId}{(userRequestCount + 1).ToString("D4")}";
            form.RequestNumber = Double.Parse(newRequestNumber);
            return form;
        }

        private void SaveDescriptions(SnapshotFormData snapshot, int requestId)

        {
            foreach (var description in snapshot.Descriptions)
            {
                _context.DescriptionEntries.Add(new DescriptionEntry
                {
                    RequestId = requestId,
                    Description1 = description.Description1,
                    Description2 = description.Description2
                });
            }
        }



        public async Task<(bool isSaved, string workflowName, string uploadsName, string documentsName)> HandleStep4Data(
          RequestViewModel data,
          List<IFormFile> workflowFiles,
          List<IFormFile> uploadsRequiredFiles,
          List<IFormFile> documentsFiles)
        {
            var snapshot = await GetOrCreateSnapshotAsync();
            var currentData = snapshot.GetFormData(_jsonOptions);

            bool anyWorkflow = workflowFiles != null && workflowFiles.Any(f => f.Length > 0);
            bool anyUploads = uploadsRequiredFiles != null && uploadsRequiredFiles.Any(f => f.Length > 0);
            bool anyDocuments = documentsFiles != null && documentsFiles.Any(f => f.Length > 0);

            if (!HasMeaningfulChangesStep4(currentData, data) && !anyWorkflow && !anyUploads && !anyDocuments)
                return (false, null, null, null);

            // Update form fields
            currentData.Workflow = data.Workflow;
            currentData.UploadsRequired = data.UploadsRequired;
            currentData.Documents = data.Documents;
            currentData.Timeline = data.Timeline;
            currentData.SystemNeeded = data.SystemNeeded;
            currentData.Cities2 = data.Cities2;
            currentData.DepartmentHeadName = data.DepartmentHeadName;
            currentData.AdditionalNotes = data.AdditionalNotes;

            snapshot.SetFormData(currentData, _jsonOptions);

            // Remove previous attachments by field
            var oldWorkflow = snapshot.Attachments.Where(a => a.FieldName == "Step4_Workflow").ToList();
            foreach (var att in oldWorkflow) snapshot.Attachments.Remove(att);

            var oldUploads = snapshot.Attachments.Where(a => a.FieldName == "Step4_uploadsRequiredFile").ToList();
            foreach (var att in oldUploads) snapshot.Attachments.Remove(att);

            var oldDocs = snapshot.Attachments.Where(a => a.FieldName == "Step4_documentsFile").ToList();
            foreach (var att in oldDocs) snapshot.Attachments.Remove(att);

            // Save new attachments
            string workflowName = null, uploadsName = null, documentsName = null;

            if (anyWorkflow)
            {
                foreach (var file in workflowFiles.Where(f => f.Length > 0))
                {
                    await HandleAttachment(file, snapshot, "Step4_Workflow");
                }
                workflowName = string.Join(", ", workflowFiles.Select(f => Path.GetFileName(f.FileName)));
            }

            if (anyUploads)
            {
                foreach (var file in uploadsRequiredFiles.Where(f => f.Length > 0))
                {
                    await HandleAttachment(file, snapshot, "Step4_uploadsRequiredFile");
                }
                uploadsName = string.Join(", ", uploadsRequiredFiles.Select(f => Path.GetFileName(f.FileName)));
            }

            if (anyDocuments)
            {
                foreach (var file in documentsFiles.Where(f => f.Length > 0))
                {
                    await HandleAttachment(file, snapshot, "Step4_documentsFile");
                }
                documentsName = string.Join(", ", documentsFiles.Select(f => Path.GetFileName(f.FileName)));
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

        public async Task AcceptTermsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var snapshot = await _context.SnapshotForms.FirstOrDefaultAsync(s => s.UserId == userId);
            if (snapshot == null)
                throw new InvalidOperationException("Snapshot not found for user.");

            snapshot.TermsAccepted = true;
            await _context.SaveChangesAsync();
        }
        public async Task<SnapshotFormData> GetCurrentSnapshotAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.SnapshotForms
                .FirstOrDefaultAsync(s => s.UserId == userId);
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

            var extension = Path.GetExtension(attachment.FileName).ToLower();

            var allowedExtensions = new[]
            {
        ".doc", ".docx", ".xls", ".xlsx", ".pdf",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Unsupported file type: {extension}");

            var existing = snapshot.Attachments
                .Where(a => a.FieldName == fieldName)
                .ToList();

            if (existing.Count >= 5)
                return; // already handled in calling method, this is a backup

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
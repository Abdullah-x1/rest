using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DSAR.Models.ViewModels
{
    public class ChangeLevelViewModel
    {
        // incoming
        public int RequestId { get; set; }

        // display only
        public int CurrentLevelId { get; set; }
        public string CurrentLevelName { get; set; }

        // bound form fields
        [Required]
        public int SelectedLevelId { get; set; }

        [Required]
        public int SelectedStatusId { get; set; }

        // dropdown sources—no binding or validation
        [BindNever]
        [ValidateNever]
        public IEnumerable<SelectListItem> Levels { get; set; }

        [BindNever]
        [ValidateNever]
        public IEnumerable<SelectListItem> Statuses { get; set; }
    }
}

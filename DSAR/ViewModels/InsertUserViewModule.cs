using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DSAR.Models.ViewModels
{
    public class InsertUserViewModel
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Display(Name = "رقم الهوية")]
        [Required] public string UserId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don’t match")]
        public string ConfirmPassword { get; set; }

        // FK dropdowns
        [Display(Name = "المدينة")]
        [Required] public int CityId { get; set; }

        [Display(Name = "القسم")]
        [Required] public int SectionId { get; set; }

        [Display(Name = "الإدارة")]
        [Required] public int DepartmentId { get; set; }

        [Display(Name = "القطاع")]
        [Required] public int SectorId { get; set; }

        [Display(Name = "الدور")]
        [Required] public string Role { get; set; }

        // these get populated in the GET action

        [ValidateNever] public IEnumerable<SelectListItem> Cities { get; set; }
        [ValidateNever] public IEnumerable<SelectListItem> Sections { get; set; }
        [ValidateNever] public IEnumerable<SelectListItem> Departments { get; set; }
        [ValidateNever] public IEnumerable<SelectListItem> Sectors { get; set; }
        [ValidateNever] public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
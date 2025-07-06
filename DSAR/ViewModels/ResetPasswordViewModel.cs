using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DSAR.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [ValidateNever]
        public string IdentityGuid { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords don’t match")]
        public string ConfirmPassword { get; set; }
    }
}

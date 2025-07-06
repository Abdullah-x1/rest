using System.Collections.Generic;

namespace DSAR.Models.ViewModels
{
    public class ListUserViewModel
    {
        // the national‐ID string, not the GUID
        public string NationalId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Identity roles 
        public IList<string> Roles { get; set; }

        // navigation properties 
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }

        public bool IsActive { get; set; }
        public int? SectorId { get; set; }

        public string CityName { get; set; }

        // real GUID Id for the “Edit” link
        public string IdentityGuid { get; set; }
    }
}

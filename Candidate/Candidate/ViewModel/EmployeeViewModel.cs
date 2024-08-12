using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candidate.ViewModel
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }  // Adding this property for Edit and Delete on Index page

        [Required (ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Mobile is required")]
        [MaxLength(10, ErrorMessage = "Mobile should be 10 digits")]
        [MinLength(10, ErrorMessage = "Mobile should be 10 digits")]
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "State is required")]
        public int? StateId { get; set; }
        [Required(ErrorMessage = "City is required")]
        public int? CityId { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }
        
        public List<int> SelectedSkillIds { get; set; } = new List<int>(); //This is to hold the Selected Skill Id
        public string? Profile { get; set; }


        [NotMapped]
        public string? StateName { get; set; } // Adding this to render on Index page only
        [NotMapped]
        public string? CityName { get; set; } // Adding this to render on Index page only


        public SelectList? States { get; set; }
        public SelectList? Cities { get; set; }
        public SelectList? Skills { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Mobile is required")]
        [MaxLength(10, ErrorMessage = "Mobile should be 10 digits")]
        [MinLength(10, ErrorMessage = "Mobile should be 10 digits")]
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "State is required")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }
        [Required(ErrorMessage = "Skill is required")]
        
        public string? Profile { get; set; }

        //Nevigation Property
        [BindNever]
        public State? State { get; set; }
        [BindNever]
        public City? City { get; set; }
        public ICollection<EmployeeSkill> EmployeeSkill { get; set; } = new List<EmployeeSkill>();
    }
}

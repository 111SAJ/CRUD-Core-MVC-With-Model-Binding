using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }
        [Required]
        public string? SkillName { get; set; }

        // Navigation properties
        public ICollection<EmployeeSkill> EmployeeSkill { get; set; } = new List<EmployeeSkill>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string? StateName { get; set; }
    }
}

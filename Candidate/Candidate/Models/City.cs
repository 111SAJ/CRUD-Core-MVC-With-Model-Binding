using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Candidate.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string? CityName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State")]
        public int StateId { get; set; } //Foregin Key

        //Navigation Property
        [BindNever]
        public State? State { get; set; }
    }
}

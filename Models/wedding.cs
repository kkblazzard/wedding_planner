using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace wedding_planner.Models

{
    public class Wedding
    {
        // auto-implemented properties need to match columns in your table
        [Key]
        public int WeddingId { get; set; }
        [Display(Name="Spouce 1 First Name:")]
        [Required]
        [MinLength(3)]
        public string name_1 {get; set;}
        [Display(Name="Spouse 2 First Name:")]
        [Required]
        [MinLength(3)]
        public string name_2 {get; set;}
        [Display(Name="Wedding Date:")]
        [Required]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage="Date should be in the future.")]
        public DateTime? date {get; set;}
        [RequiredAttribute]
        public int creatorId {get; set;}
        public string address {get; set;}
        public DateTime CreatedAt {get; set;}=DateTime.Now;
        public DateTime UpdatedAt {get; set;}=DateTime.Now;
        public List<RSVP> RSVP {get; set;}
    }
}
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        return value != null && (DateTime)value > DateTime.Now;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace wedding_planner.Models
{
    public class RSVP
    {
        // auto-implemented properties need to match columns in your table
        [Key]
        public int RsvpId { get; set;}
        public int UserId {get; set;}
        public User User {get; set;}
        public int WeddingId {get; set;}
        public Wedding Wedding {get; set;}
        
    }
}

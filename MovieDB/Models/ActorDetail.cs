using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class ActorDetail
    {
        public ActorDetail() { }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        //Lägg till max characters 50. 
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Födelseår")]
        public int BirthYear { get; set; }
    }
}

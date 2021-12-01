using Microsoft.AspNetCore.Http;
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
        [StringLength(30, ErrorMessage = "Förnamn får vara max 30 karaktärer lång", MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        [StringLength(30, ErrorMessage = "Efternamn får vara max 30 karaktärer lång", MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Födelseår")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Ange ett årtal")]
        public int BirthYear { get; set; }

        [Display(Name = "Filmer")]
        public List<String> Movies { get; set; }

        [Display(Name ="Bild")]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}

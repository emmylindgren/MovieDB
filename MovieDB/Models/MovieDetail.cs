using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class MovieDetail
    {
        public MovieDetail() {}

        public int Id { get; set; }

        [Required]
        [Display(Name = "Titel")]
        [StringLength(50, ErrorMessage = "Titeln får vara max 50 karaktärer lång", MinimumLength=1)]
        public string Title { get; set; }
        

        [Required]
        [Display(Name = "Utgivningsår")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Ange ett årtal")]
        public int ReleaseYear { get; set; }

        [Required]
        [Display(Name = "Språk")]
        public String OnLanguage { get; set; }

        [Required]
        [Display(Name = "Betyg (1-5)")]
        [Range(1, 5)]
        public int Grade { get; set; }

        [Required]
        [Display(Name = "Genre")]
        public String Genre { get; set; }

        [Display(Name = "Skådespelare")]
        public List<String> Actors { get; set; }

        public int removeAllActors()
        {
            Actors.Clear();
            return 0;
        }
    }

}

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2025_Project3_snkyemba.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        [Range(1888, 2035, ErrorMessage = "Year must be between 1888 and 2035")]
        public int Year { get; set; }

        [Display(Name = "IMDB Link")]
        public string IMDBLink { get; set; }

        public byte[]? Poster { get; set; }

        public ICollection<ActorMovie>? ActorMovies { get; set; }
    }
}

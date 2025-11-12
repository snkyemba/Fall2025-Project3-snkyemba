using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_snkyemba.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }

        [Display(Name = "IMDB Link")]
        public string IMDBLink { get; set; }

        public byte[]? Photo { get; set; }

        public ICollection<ActorMovie>? ActorMovies { get; set; }
    }
}

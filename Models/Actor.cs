using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_snkyemba.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Gender { get; set; }

        [Range(0, 122, ErrorMessage = "Age must be between 0 and 122")]
        public int Age { get; set; }

        [Display(Name = "IMDB Link")]
        public string IMDBLink { get; set; }

        public byte[]? Photo { get; set; }

        public ICollection<ActorMovie>? ActorMovies { get; set; }
    }
}

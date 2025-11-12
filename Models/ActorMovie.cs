//using all2025_Project3_snkyemba.Models;
using Fall2025_Project3_snkyemba.Models;

namespace Fall2025_Project3_snkyemba.Models
{
    public class ActorMovie
    {
        public int Id { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}

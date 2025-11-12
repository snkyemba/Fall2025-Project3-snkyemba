using Fall2025_Project3_snkyemba.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fall2025_Project3_snkyemba.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for your models
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }

        // Optional: configure many-to-many relationships
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Must call this for Identity tables

            // Example: configure composite key for ActorMovie
            builder.Entity<ActorMovie>()
                .HasKey(am => new { am.ActorId, am.MovieId });

            builder.Entity<ActorMovie>()
                .HasOne(am => am.Actor)
                .WithMany(a => a.ActorMovies)
                .HasForeignKey(am => am.ActorId);

            builder.Entity<ActorMovie>()
                .HasOne(am => am.Movie)
                .WithMany(m => m.ActorMovies)
                .HasForeignKey(am => am.MovieId);
        }
    }
}

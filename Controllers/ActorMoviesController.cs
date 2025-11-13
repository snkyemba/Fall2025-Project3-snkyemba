using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_snkyemba.Data;
using Fall2025_Project3_snkyemba.Models;

namespace Fall2025_Project3_snkyemba.Controllers
{
    public class ActorMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActorMovies
        public async Task<IActionResult> Index()
        {
            var actorMovies = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .ToListAsync();
            return View(actorMovies);
        }

        // GET: ActorMovies/Create
        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name");
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return View();
        }

        // POST: ActorMovies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorMovie actorMovie)
        {
            // Remove navigation properties from validation
            ModelState.Remove("Actor");
            ModelState.Remove("Movie");

            // Check if relationship already exists
            var exists = await _context.ActorMovies
                .AnyAsync(am => am.ActorId == actorMovie.ActorId && am.MovieId == actorMovie.MovieId);

            if (exists)
            {
                ModelState.AddModelError("", "This actor is already associated with this movie.");
                ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", actorMovie.ActorId);
                ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", actorMovie.MovieId);
                return View(actorMovie);
            }

            if (ModelState.IsValid)
            {
                _context.Add(actorMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", actorMovie.MovieId);
            return View(actorMovie);
        }

        // GET: ActorMovies/Delete/5
        public async Task<IActionResult> Delete(int? actorId, int? movieId)
        {
            if (actorId == null || movieId == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .FirstOrDefaultAsync(m => m.ActorId == actorId && m.MovieId == movieId);

            if (actorMovie == null)
            {
                return NotFound();
            }

            return View(actorMovie);
        }

        // POST: ActorMovies/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int actorId, int movieId)
        {
            var actorMovie = await _context.ActorMovies
                .FirstOrDefaultAsync(m => m.ActorId == actorId && m.MovieId == movieId);

            if (actorMovie != null)
            {
                _context.ActorMovies.Remove(actorMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
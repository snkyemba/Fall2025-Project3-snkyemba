using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_snkyemba.Data;
using Fall2025_Project3_snkyemba.Services;
using Fall2025_Project3_snkyemba.ViewModels;
using Fall2025_Project3_snkyemba.Models;
using VaderSharp2;

namespace Fall2025_Project3_snkyemba.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AiService _aiService;

        // Fixed constructor - inject AiService properly
        public MoviesController(ApplicationDbContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.ActorMovies)
                .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            // Generate reviews using AI
            var reviews = await _aiService.GenerateMovieReviewsAsync(movie.Title);

            // Analyze sentiment for each review
            var analyzer = new SentimentIntensityAnalyzer();
            var reviewSentiments = new List<SentimentAnalysisResults>();

            foreach (var review in reviews)
            {
                var result = analyzer.PolarityScores(review);
                reviewSentiments.Add(result);
            }

            // Calculate average sentiment
            var avgSentiment = reviewSentiments.Any()
                ? reviewSentiments.Average(s => s.Compound)
                : 0;

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                ReviewSentiments = reviewSentiments,
                AverageSentiment = avgSentiment
            };

            return View(viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile PosterFile)
        {
            // Remove Poster from ModelState since we handle file upload separately
            ModelState.Remove("Poster");

            if (PosterFile != null && PosterFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PosterFile.CopyToAsync(ms);
                movie.Poster = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? PosterFile)
        {
            if (id != movie.Id) return NotFound();

            // Fetch the original movie from DB
            var movieInDb = await _context.Movies.FindAsync(id);
            if (movieInDb == null) return NotFound();

            // Update fields that come from the form
            movieInDb.Title = movie.Title;
            movieInDb.IMDBLink = movie.IMDBLink;
            movieInDb.Genre = movie.Genre;
            movieInDb.Year = movie.Year;

            // Only update Poster if a new file was uploaded
            if (PosterFile != null && PosterFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PosterFile.CopyToAsync(ms);
                movieInDb.Poster = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movieInDb);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
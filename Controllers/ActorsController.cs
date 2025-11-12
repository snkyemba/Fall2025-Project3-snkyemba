using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_snkyemba.Data;
using Fall2025_Project3_snkyemba.Models;
using Fall2025_Project3_snkyemba.Services;
using Fall2025_Project3_snkyemba.ViewModels;
using VaderSharp2;

namespace Fall2025_Project3_snkyemba.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AiService _aiService;

        // Fixed constructor - inject AiService properly
        public ActorsController(ApplicationDbContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.ActorMovies)
                .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            // Generate tweets using AI
            var tweets = await _aiService.GenerateActorTweetsAsync(actor.Name);

            // Analyze sentiment for each tweet
            var analyzer = new SentimentIntensityAnalyzer();
            var tweetSentiments = new List<SentimentAnalysisResults>();

            foreach (var tweet in tweets)
            {
                var result = analyzer.PolarityScores(tweet);
                tweetSentiments.Add(result);
            }

            // Calculate average sentiment (using Compound score)
            var overallSentiment = tweetSentiments.Any()
                ? tweetSentiments.Average(s => s.Compound)
                : 0;

            var viewModel = new ActorDetailsViewModel
            {
                Actor = actor,
                Tweets = tweets,
                TweetSentiments = tweetSentiments,
                OverallSentiment = overallSentiment
            };

            return View(viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile PhotoFile)
        {
            // Remove Photo from ModelState since we handle file upload separately
            ModelState.Remove("Photo");

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                actor.Photo = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? PhotoFile)
        {
            if (id != actor.Id) return NotFound();

            // Fetch the original actor from DB
            var actorInDb = await _context.Actors.FindAsync(id);
            if (actorInDb == null) return NotFound();

            // Update fields that come from the form
            actorInDb.Name = actor.Name;
            actorInDb.Gender = actor.Gender;
            actorInDb.Age = actor.Age;
            actorInDb.IMDBLink = actor.IMDBLink;

            // Only update Photo if a new file was uploaded
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                actorInDb.Photo = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actorInDb);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
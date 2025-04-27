using camping_rating.Data;
using camping_rating.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace camping_rating.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin view
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.CampingSpot)
                .ToList();
            return View(reviews);
        }

        // User's reviews
        public async Task<IActionResult> MyReviews()
        {
            var user = await _userManager.GetUserAsync(User);
            var reviews = _context.Reviews
                .Where(r => r.UserId == user.Id)
                .Include(r => r.CampingSpot)
                .ToList();
            return View(reviews);
        }

        public IActionResult Create(int campingSpotId)
        {
            var model = new Review { CampingSpotId = campingSpotId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review model)
        {
            // Set user ID before validation
            var user = await _userManager.GetUserAsync(User);
            model.UserId = user.Id;
            model.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyReviews");
            }

            // If invalid, preserve CampingSpotId
            ViewBag.CampingSpotId = model.CampingSpotId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (review.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(User.IsInRole("Admin") ? "Index" : "MyReviews");
        }
    }
}

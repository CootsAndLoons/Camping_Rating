using camping_rating.Data;
using camping_rating.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace camping_rating.Controllers
{
    [Authorize]
    public class CampingSpotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CampingSpotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var spots = _context.CampingSpots.ToList();
            return View(spots);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CampingSpot model, IFormFile photo)
        {
            if (photo != null && photo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    model.Photo = memoryStream.ToArray();
                }
            }
            else
            {
                model.Photo = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var spot = await _context.CampingSpots.FindAsync(id);
            if (spot == null) return NotFound();
            return View(spot);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CampingSpot model, IFormFile photo)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var existingSpot = await _context.CampingSpots.FindAsync(id);
            if (existingSpot == null) return NotFound();

            // Preserve existing photo if no new upload
            if (photo == null || photo.Length == 0)
            {
                model.Photo = existingSpot.Photo;
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    model.Photo = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                _context.Entry(existingSpot).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var spot = await _context.CampingSpots.FindAsync(id);
            if (spot == null) return NotFound();

            _context.CampingSpots.Remove(spot);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

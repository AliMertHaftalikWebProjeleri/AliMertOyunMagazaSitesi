using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AliMertOyunMagaza.Data;

namespace AliMertOyunMagaza.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var gamesQuery = _context.Games.Include(g => g.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.CategoryId == categoryId.Value);
                ViewBag.SelectedCategory = await _context.Categories.FindAsync(categoryId.Value);
            }

            return View(await gamesQuery.ToListAsync());
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AliMertOyunMagaza.Models;
using AliMertOyunMagaza.Data;

namespace AliMertOyunMagaza.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Game)
                .ThenInclude(g => g.Category)
                .Where(w => w.UserId == user.Id)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int gameId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var exists = await _context.WishlistItems.AnyAsync(w => w.GameId == gameId && w.UserId == user.Id);
            if (!exists)
            {
                _context.WishlistItems.Add(new WishlistItem
                {
                    GameId = gameId,
                    UserId = user.Id
                });
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Oyun istek listesine eklendi.";
            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int wishlistId)
        {
            var user = await _userManager.GetUserAsync(User);
            var item = await _context.WishlistItems.FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == user.Id);
            
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Oyun istek listesinden çıkarıldı.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce535.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} ,  {CD.ADMIN_ROLE}")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users);
        }
       public async Task<IActionResult> LockUnLock(string id )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            if (user.LockoutEnd == null || DateTime.UtcNow > user.LockoutEnd)
            {
                await _userManager.SetLockoutEndDateAsync(user , DateTime.UtcNow.AddYears(1));  
            }
            else 
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            return RedirectToAction(nameof(Index)); 
        }
    }
}

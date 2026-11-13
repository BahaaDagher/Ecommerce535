using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster; 

namespace Ecommerce535.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); 
            if(user is null) return NotFound();

            // manual mapping 


            //var applicationUserVm = new ApplicationUserVM();
            //applicationUserVm.Name = user.Name; 
            //applicationUserVm.Address = user.Address; 
            //applicationUserVm.PhoneNumber = user.PhoneNumber; 
            //applicationUserVm.Email = user.Email; 

            var applicationUserVm = user.Adapt<ApplicationUserVM>(); 


            return View(applicationUserVm);
        }
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            user.Name = applicationUserVM.Name;
            user.PhoneNumber = applicationUserVM.PhoneNumber; 
            user.Address = applicationUserVM.Address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors; 
            }
            else
            {
                TempData["Successful_Notification"] = "user updated successfully"; 
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var result =  await _userManager.ChangePasswordAsync(user , applicationUserVM.CurrentPassword , applicationUserVM.NewPassword);


            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update password successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using REVAACOURSES.Models;
using REVAACOURSES.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Identity.Controllers
{
    [Area("Identity")]
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

            if (user == null)
            {
                return NotFound();
            }

            var applicationUserVM = user.Adapt<ApplicationUserVM>();

            return View(applicationUserVM);
        }

        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = applicationUserVM.Name;
            user.Address = applicationUserVM.Address;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error-Notification"] = errors;

                return View(nameof(Index), applicationUserVM);
            }
            else
            {
                TempData["Success-Notification"] = "Your profile updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error-Notification"] = errors;

                return View(nameof(Index), applicationUserVM);
            }
            else
            {
                TempData["Success-Notification"] = "Your password updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

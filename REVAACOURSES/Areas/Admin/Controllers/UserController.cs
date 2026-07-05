using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using REVAACOURSES.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.ASSISTANT_ROLE},{CD.INSTRUCTOR_ROLE}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Assistant> _assistantRepository;
        private readonly IRepository<Instructor> _instructorRepository;

        public UserController(UserManager<ApplicationUser> userManager, IRepository<Student> studentRepository, IRepository<Assistant> assistantRepository, IRepository<Instructor> instructorRepository)
        {
            _userManager = userManager;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _instructorRepository = instructorRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            List<UserVM> usersVM = new();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersVM.Add(new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList()
                });
            }
            return View(usersVM);
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            bool isSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN_ROLE);
            if (isSuperAdmin)
            {
                TempData["Error-Notification"] = "You cannot lock/unlock a Super Admin user.";
                return RedirectToAction(nameof(Index));
            }

            if (user.LockoutEnd is null || DateTime.UtcNow > user.LockoutEnd)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(3));
                TempData["Success-Notification"] = "User locked successfully";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success-Notification"] = "User unlocked successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> ChangeRole(string id, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool isSuperAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    CD.SUPER_ADMIN_ROLE
                );

            if (isSuperAdmin)
            {
                TempData["Error-Notification"] =
                    "You cannot change Super Admin role";

                return RedirectToAction(nameof(Index));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                 _studentRepository.DeleteAsync(student);
            }

            var instructor = await _instructorRepository.GetOneAsync(i => i.UserId == user.Id);

            if (instructor != null)
            {
                 _instructorRepository.DeleteAsync(instructor);
            }

            var assistant = await _assistantRepository.GetOneAsync(a => a.UserId == user.Id);

            if (assistant != null)
            {
                 _assistantRepository.DeleteAsync(assistant);
            }

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (roles.Any())
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            if (roles.Contains(CD.STUDENT_ROLE))
            {
                var studentExists =
                    await _studentRepository.GetOneAsync(
                        s => s.UserId == user.Id);

                if (studentExists == null)
                {
                    await _studentRepository.AddAsync(
                        new Student
                        {
                            UserId = user.Id
                        });
                }
            }

            if (roles.Contains(CD.INSTRUCTOR_ROLE))
            {
                var instructorExists =
                    await _instructorRepository.GetOneAsync(
                        i => i.UserId == user.Id);

                if (instructorExists == null)
                {
                    await _instructorRepository.AddAsync(
                        new Instructor
                        {
                            UserId = user.Id
                        });
                }
            }

            if (roles.Contains(CD.ASSISTANT_ROLE))
            {
                var assistantExists =
                    await _assistantRepository.GetOneAsync(
                        a => a.UserId == user.Id);

                if (assistantExists == null)
                {
                    await _assistantRepository.AddAsync(
                        new Assistant
                        {
                            UserId = user.Id
                        });
                }
            }

            await _studentRepository.CommitAsync();
            await _assistantRepository.CommitAsync();
            await _instructorRepository.CommitAsync();

            TempData["Success-Notification"] =
                "Roles updated successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
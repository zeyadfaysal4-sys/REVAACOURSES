using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using REVAACOURSES.ViewModels;
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
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Instructor> _instructorRepository;

        public UserController(UserManager<ApplicationUser> userManager, IRepository<Student> studentRepository, IRepository<Assistant> assistantRepository, IRepository<Instructor> instructorRepository, IEmailSender emailSender)
        {
            _userManager = userManager;
            _studentRepository = studentRepository;
            _assistantRepository = assistantRepository;
            _instructorRepository = instructorRepository;
            _emailSender = emailSender;
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

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View(new CreateEmployeeVM());
        }

        [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeVM employeeVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error-Notification"] = "Invalid Data";
                return View(employeeVM);
            }

            var employee = new ApplicationUser()
            {
                UserName = employeeVM.UserName,
                Email = employeeVM.Email,
                Name = employeeVM.Name,
                PhoneNumber = employeeVM.PhoneNumber,
                Address = employeeVM.Address
            };
            var result = await _userManager.CreateAsync(employee, employeeVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(employeeVM);
            }
            await _userManager.AddToRoleAsync(employee, employeeVM.Role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(employee);

            var link = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    area = "Identity",
                    userId = employee.Id,
                    token = token
                },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                employee.Email,
                "Confirm Your Email",
                $"<h1>Click <a href='{link}'>here</a> to confirm your email.</h1>");


            if (employeeVM.Role == CD.INSTRUCTOR_ROLE)
            {
                await _instructorRepository.AddAsync(new Instructor
                {
                    UserId = employee.Id,
                    Bio = employeeVM.Bio
                });

                await _instructorRepository.CommitAsync();
            }

            if (employeeVM.Role == CD.ASSISTANT_ROLE)
            {
                await _assistantRepository.AddAsync(new Assistant
                {
                    UserId = employee.Id
                });

                await _assistantRepository.CommitAsync();
            }

            TempData["Success-Notification"] = "Employee created successfully";

            return RedirectToAction(nameof(Index));

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

        [HttpGet]
        [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UserVM
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return View(vm);
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
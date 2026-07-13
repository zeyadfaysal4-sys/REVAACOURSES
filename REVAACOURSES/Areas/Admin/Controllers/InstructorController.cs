using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]
    public class InstructorController : Controller
    {
        private readonly IRepository<Instructor> _instructorRepository;

        public InstructorController(IRepository<Instructor> instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }

        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorRepository.GetAsync(includes: [i => i.User]);
            return View(instructors.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var instructor = await _instructorRepository.GetOneAsync(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Instructor instructor)
        {

            if (ModelState.IsValid)
            {
                return View(instructor);
            }

            _instructorRepository.UpdateAsync(instructor);
            await _instructorRepository.CommitAsync();

            TempData["Success-Notification"] = "Instructor updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var instructor = await _instructorRepository.GetOneAsync(i => i.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }
            _instructorRepository.DeleteAsync(instructor);
            await _instructorRepository.CommitAsync();
            TempData["Success-Notification"] = "Instructor deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

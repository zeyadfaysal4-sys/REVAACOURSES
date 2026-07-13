using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.INSTRUCTOR_ROLE}")]

    public class LessonController : Controller
    {
        private readonly IRepository<Instructor> _instructorRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Course> _courseRepository;


        public LessonController(IRepository<Instructor> instructorRepository, IRepository<Lesson> lessonRepository, IRepository<Course> courseRepository)
        {
            _instructorRepository = instructorRepository;
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index()
        {
            var lessons = await _lessonRepository.GetAsync(includes: [l => l.Course]);
            return View(lessons);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _courseRepository.GetAsync();

            return View(new Lesson());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Courses = await _courseRepository.GetAsync();
                return View(lesson);
            }

            await _lessonRepository.AddAsync(lesson);
            await _lessonRepository.CommitAsync();

            TempData["Success-Notification"] = "Lesson created successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == id);
            if (lesson is null)
            {
                return NotFound();
            }
            ViewBag.Courses = await _courseRepository.GetAsync();
            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Lesson lesson)
        {

            if (ModelState.IsValid)
            {
                return View(lesson);
            }

            ViewBag.Courses = await _courseRepository.GetAsync();
            _lessonRepository.UpdateAsync(lesson);
            await _lessonRepository.CommitAsync();

            TempData["Success-Notification"] = "Lesson updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == id);
            if (lesson is null)
            {
                return NotFound();
            }

            _lessonRepository.DeleteAsync(lesson);
            await _lessonRepository.CommitAsync();
            TempData["Success-Notification"] = "Lesson deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

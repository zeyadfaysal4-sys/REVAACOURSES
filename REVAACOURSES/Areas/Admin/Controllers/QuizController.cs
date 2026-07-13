using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.INSTRUCTOR_ROLE}")]
    public class QuizController : Controller
    {
        private readonly IRepository<Quiez> _quizRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Course> _courseRepository;

        public QuizController(IRepository<Quiez> quizRepository, IRepository<Lesson> lessonRepository, IRepository<Course> courseRepository)
        {
            _quizRepository = quizRepository;
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index()
        {

            var quizzes = await _quizRepository.GetAsync(includes: [q => q.Lesson]);
            return View(quizzes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.Courses = await _courseRepository.GetAsync();
            ViewBag.Lessons = await _lessonRepository.GetAsync();

            return View(new Quiez());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Quiez quiz)
        {

            if (ModelState.IsValid)
            {
                ViewBag.Lessons = await _lessonRepository.GetAsync();
                return View(quiz);
            }

            quiz.CreatedAt = DateTime.Now;

            await _quizRepository.AddAsync(quiz);
            await _quizRepository.CommitAsync();

            TempData["Success-Notification"] = "Quiz created successfully";


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var quiz = await _quizRepository.GetOneAsync(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }
            ViewBag.Lessons = await _lessonRepository.GetAsync();
            ViewBag.Courses = await _courseRepository.GetAsync();

            return View(quiz);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Quiez quiz)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Lessons = await _lessonRepository.GetAsync();
                return View(quiz);
            }
  
            _quizRepository.UpdateAsync(quiz);
            await _quizRepository.CommitAsync();

            TempData["Success-Notification"] = "Quiz updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            ViewBag.Lessons = await _lessonRepository.GetAsync();

            var quiz = await _quizRepository.GetOneAsync(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            _quizRepository.DeleteAsync(quiz);
            await _quizRepository.CommitAsync();

            TempData["Success-Notification"] = "Quiz deleted successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}

        

    


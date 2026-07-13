using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE}, {CD.SUPER_ADMIN_ROLE}, {CD.INSTRUCTOR_ROLE}")]
    public class QuestionController : Controller
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Quiez> _quizRepository;
        private readonly IRepository<Lesson> _lessonRepository;


        public QuestionController(IRepository<Question> questionRepository, IRepository<Quiez> quizRepository, IRepository<Lesson> lessonRepository)
        {
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<IActionResult> Index()
        {

            var questions = await _questionRepository.GetAsync(includes: [q => q.Quiz]);
            return View(questions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.Lessons = await _lessonRepository.GetAsync();
            ViewBag.Quizzes = await _quizRepository.GetAsync();

            return View(new Question());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Lessons = await _lessonRepository.GetAsync();
                ViewBag.Quizzes = await _quizRepository.GetAsync();
                return View(question);
            }

            var quation = await _questionRepository.AddAsync(question);
            await _questionRepository.CommitAsync();

            TempData["Success-Notification"] = "Question created successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var question = await _questionRepository.GetOneAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.Lessons = await _lessonRepository.GetAsync();
            ViewBag.Quizzes = await _quizRepository.GetAsync();

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Question question)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Lessons = await _lessonRepository.GetAsync();
                ViewBag.Quizzes = await _quizRepository.GetAsync();
                return View(question);
            }

            _questionRepository.UpdateAsync(question);
            await _questionRepository.CommitAsync();
            TempData["Success-Notification"] = "Question updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionRepository.GetOneAsync(q => q.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            _questionRepository.DeleteAsync(question);
            await _questionRepository.CommitAsync();
            TempData["Success-Notification"] = "Question deleted successfully!";

            return RedirectToAction(nameof(Index));

        }
    }
}

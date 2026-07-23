using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.ViewModels;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class MyLearningController : Controller
    {
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Quiez> _quizRepository;
        private readonly IRepository<StudentProgress> _progressRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        public MyLearningController(IRepository<Enrollment> enrollmentRepository, IRepository<Course> courseRepository, UserManager<ApplicationUser> userManager, IRepository<Student> studentRepository, IRepository<Lesson> lessonRepository, IRepository<Quiez> quizRepository, IRepository<StudentProgress> progressRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _studentRepository = studentRepository;
            _lessonRepository = lessonRepository;
            _quizRepository = quizRepository;
            _progressRepository = progressRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error-Notification"] = "You need to register as a student to access your learning dashboard.";
                return RedirectToAction("Register", "Account", new { area = "Identity" });
            }

            var enrollments = await _enrollmentRepository.GetAsync(e => e.StudentId == student.Id, includes: [e => e.Course]);
            return View(enrollments);
        }

        public async Task<IActionResult> Lessons(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student == null)
                return NotFound();

            var course = await _courseRepository.GetOneAsync(c => c.Id == courseId);

            if (course == null)
                return NotFound();

            var enrollment = await _enrollmentRepository.GetOneAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

            if (enrollment == null)
            {
                return Forbid();
            }

            var lessons = await _lessonRepository.GetAsync(l => l.CourseId == courseId);

            var progress = await _progressRepository.GetAsync(p => p.StudentId == student.Id);

            var quizzes = await _quizRepository.GetAsync();

            ViewBag.CourseTitle = course.Title;
            ViewBag.Progress = progress;
            ViewBag.Quizzes = quizzes;

            return View(lessons);
        }

        public async Task<IActionResult> Watch(int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student == null)
                return NotFound();
            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }
            var enrollment = await _enrollmentRepository.GetOneAsync(e =>e.StudentId == student.Id &&e.CourseId == lesson.CourseId);

            if (enrollment == null)
            {
                return Forbid();
            }
            var quiz = await _quizRepository.GetOneAsync(q => q.LessonId == lessonId);
            ViewBag.QuizId = quiz?.Id;
            return View(lesson);
        }
    }
}

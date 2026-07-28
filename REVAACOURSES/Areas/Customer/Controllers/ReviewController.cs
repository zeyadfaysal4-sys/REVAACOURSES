using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<StudentProgress> _progressRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public ReviewController(IRepository<Review> reviewRepository, IRepository<Student> studentRepository, IRepository<Course> courseRepository, IRepository<Enrollment> enrollmentRepository, UserManager<ApplicationUser> userManager, IRepository<StudentProgress> progressRepository, IRepository<Lesson> lessonRepository)
        {
            _reviewRepository = reviewRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _userManager = userManager;
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student is null)
            {
                return NotFound();
            }

            var enrollment = await _enrollmentRepository.GetOneAsync(e => e.StudentId == student.Id && e.CourseId == courseId);
            if (enrollment is null)
            {
                return NotFound();
            }
            var totalLessons = (await _lessonRepository.GetAsync(l => l.CourseId == courseId)).Count();

            var completedLessons = (await _progressRepository.GetAsync(
                p => p.StudentId == student.Id,
                includes: [p => p.Lesson]))
                .Count(p => p.IsCompleted && p.Lesson.CourseId == courseId);

            if (totalLessons == 0 || completedLessons != totalLessons)
            {
                TempData["Error-Notification"] =
                    "Complete all lessons before leaving a review.";

                return RedirectToAction("Lessons", "MyLearning", new { courseId });
            }

            var existingReview = await _reviewRepository.GetOneAsync(r => r.CourseId == courseId && r.StudentId == student.Id);
            if (existingReview != null)
            {
                TempData["Error-Notification"] = "You have already reviewed this course.";

                return RedirectToAction("Lessons", "MyLearning", new { courseId });
            }
            return View(new Review { CourseId = courseId });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student is null)
            {
                return NotFound();
            }


            var enrollment = await _enrollmentRepository.GetOneAsync(s=>s.StudentId== student.Id&& s.CourseId == review.CourseId);
            if (enrollment is null)
            {
                return NotFound();
            }

            var totalLessons = (await _lessonRepository.GetAsync(l => l.CourseId == review.CourseId)) .Count();

            var completedLessons = (await _progressRepository.GetAsync(
                p => p.StudentId == student.Id,includes:
                [p => p.Lesson])).Count(p => p.IsCompleted && p.Lesson.CourseId == review.CourseId);

            if (totalLessons == 0 || completedLessons != totalLessons)
            {
                TempData["Error-Notification"] =
                    "Complete all lessons before leaving a review.";

                return RedirectToAction(
                    "Lessons",
                    "MyLearning",
                    new { courseId = review.CourseId });
            }


            var existingReview = await _reviewRepository.GetOneAsync(s => s.StudentId == student.Id && s.CourseId == review.CourseId);
            if (existingReview != null)
            {
                TempData["Error-Notification"] = "You have already reviewed this course.";
                return RedirectToAction("Lessons", "MyLearning", new { courseId = review.CourseId });
            }


            if (ModelState.IsValid)
            {
                return View(review);
            }


            review.StudentId = student.Id;
            review.CreatedAt = DateTime.Now;

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.CommitAsync();

            TempData["Success-Notification"] ="Your review has been submitted successfully.";
            
            return RedirectToAction("Lessons","MyLearning",new { courseId = review.CourseId });
        
        }   
    }
}

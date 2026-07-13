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
    public class QuizController : Controller
    {
        private readonly IRepository<Quiez> _quizRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuizResult> _quizResultRepository;
        private readonly IRepository<StudentProgress> _progressRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Certificate> _certificateRepository;
        public QuizController(IRepository<Quiez> quizRepository, IRepository<Student> studentRepository, UserManager<ApplicationUser> userManager, IRepository<Question> questionRepository, IRepository<QuizResult> quizResultRepository, IRepository< StudentProgress> progressRepository, IRepository<Lesson> lessonRepository, IRepository<Certificate> certificateRepository)
        {
            _quizRepository = quizRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
            _questionRepository = questionRepository;
            _quizResultRepository = quizResultRepository;
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
            _certificateRepository = certificateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Start(int id)
        {
            var quiz = await _quizRepository.GetOneAsync(
                q => q.Id == id,
                includes: [q => q.Questions]);

            if (quiz == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student == null)
                return NotFound();

            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == quiz.LessonId);

            if (lesson == null)
                return NotFound();

            var result = await _quizResultRepository.GetOneAsync(r =>
                r.StudentId == student.Id &&
                r.QuizId == quiz.Id);

            if (result != null)
            {
                TempData["Error-Notification"] = "You have already submitted this quiz.";

                return RedirectToAction("Lessons", "MyLearning", new
                {
                    courseId = lesson.CourseId
                });
            }

            return View(quiz);
        }
        [HttpPost]
        public async Task<IActionResult> Submit(QuizSubmissionVM model)
        {
            var quiz = await _quizRepository.GetOneAsync(q => q.Id == model.QuizId,includes: [q => q.Questions]);

            if (quiz == null)
            {
                return NotFound();
            }

            int score = 0;

            foreach (var question in quiz.Questions)
            {
                if (model.Answers.TryGetValue(question.Id, out var answer))
                {
                    if (answer == question.CorrectAnswer.ToString())
                    {
                        score++;
                    }
                }
            }

            var user1 = await _userManager.GetUserAsync(User);

            if (user1 == null)
            {
                return NotFound();
            }

            var student1 = await _studentRepository.GetOneAsync(s => s.UserId == user1.Id);

            if (student1 == null)
            {
                return NotFound();
            }

            // هل الطالب حل الكويز قبل كده؟
            var result = await _quizResultRepository.GetOneAsync(r =>
                r.StudentId == student1.Id &&
                r.QuizId == quiz.Id);

            if (result == null)
            {
                await _quizResultRepository.AddAsync(new QuizResult
                {
                    StudentId = student1.Id,
                    QuizId = quiz.Id,
                    Score = score,
                    Passed = score == quiz.Questions.Count,
                    SubmittedAt = DateTime.Now
                });

                await _quizResultRepository.CommitAsync();
            }

            if (score == quiz.Questions.Count)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

                    if (student != null)
                    {
                        var lesson = await _lessonRepository.GetOneAsync(l => l.Id == quiz.LessonId);

                        if (lesson != null)
                        {
                            // تسجيل إنه خلص الدرس
                            var progress = await _progressRepository.GetOneAsync(p =>
                                p.StudentId == student.Id &&
                                p.LessonId == lesson.Id);

                            if (progress == null)
                            {
                                await _progressRepository.AddAsync(new StudentProgress
                                {
                                    StudentId = student.Id,
                                    LessonId = lesson.Id,
                                    IsCompleted = true,
                                    CompletedAt = DateTime.Now
                                });

                                await _progressRepository.CommitAsync();
                            }

                            // جميع دروس الكورس
                            var allLessons = await _lessonRepository.GetAsync(l =>
                                l.CourseId == lesson.CourseId);

                            // الدروس المكتملة للطالب
                            var completedLessons = await _progressRepository.GetAsync(p =>
                                p.StudentId == student.Id &&
                                p.IsCompleted);

                            // هل خلص الكورس؟
                            bool finishedCourse = allLessons.All(l =>
                                completedLessons.Any(p => p.LessonId == l.Id));

                            if (finishedCourse)
                            {
                                var certificate = await _certificateRepository.GetOneAsync(c =>
                                    c.StudentId == student.Id &&
                                    c.CourseId == lesson.CourseId);

                                if (certificate == null)
                                {
                                    await _certificateRepository.AddAsync(new Certificate
                                    {
                                        StudentId = student.Id,
                                        CourseId = lesson.CourseId,
                                        CertificateNumber = "CERT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                                    });

                                    await _certificateRepository.CommitAsync();
                                }
                            }
                        }
                    }
                }
            }

            ViewBag.Score = score;
            ViewBag.Total = quiz.Questions.Count;

            return View("Result");
        }
    }
}

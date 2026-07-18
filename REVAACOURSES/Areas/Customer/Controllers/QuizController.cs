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
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuizResult> _quizResultRepository;
        private readonly IRepository<StudentProgress> _progressRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Certificate> _certificateRepository;
        public QuizController(IRepository<Quiez> quizRepository, IRepository<Student> studentRepository, IRepository<Enrollment> enrollmentRepository, UserManager<ApplicationUser> userManager, IRepository<Question> questionRepository, IRepository<QuizResult> quizResultRepository, IRepository< StudentProgress> progressRepository, IRepository<Lesson> lessonRepository, IRepository<Certificate> certificateRepository)
        {
            _quizRepository = quizRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
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
            var quiz = await _quizRepository.GetOneAsync(q => q.Id == id, includes: [q => q.Questions]);

            if (quiz == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                return NotFound();
            }

            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == quiz.LessonId);

            if (lesson == null)
            {
                return NotFound();
            }
            var enrollment = await _enrollmentRepository.GetOneAsync(e => e.StudentId == student.Id & e.CourseId == lesson.CourseId);

            if (enrollment == null)
            {
                return Forbid();
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

            await _quizResultRepository.AddAsync(new QuizResult
            {
                StudentId = student1.Id,
                QuizId = quiz.Id,
                Score = score,
                Passed = score == quiz.Questions.Count,
                SubmittedAt = DateTime.Now
            });

            await _quizResultRepository.CommitAsync();

            var lesson = await _lessonRepository.GetOneAsync(l => l.Id == quiz.LessonId);

            if (lesson != null)
            {
                var progress = await _progressRepository.GetOneAsync(p =>
                    p.StudentId == student1.Id &&
                    p.LessonId == lesson.Id);

                if (progress == null)
                {
                    await _progressRepository.AddAsync(new StudentProgress
                    {
                        StudentId = student1.Id,
                        LessonId = lesson.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.Now
                    });

                    await _progressRepository.CommitAsync();
                }

                // إصدار الشهادة فقط إذا الطالب نجح في الكويز
                if (score == quiz.Questions.Count)
                {
                    // جميع دروس الكورس
                    var allLessons = await _lessonRepository.GetAsync(l => l.CourseId == lesson.CourseId);

                    // الدروس المكتملة للطالب
                    var completedLessons = await _progressRepository.GetAsync(p =>
                        p.StudentId == student1.Id && p.IsCompleted);

                    // هل أكمل كل الدروس؟
                    bool finishedCourse = allLessons.All(l =>
                        completedLessons.Any(p => p.LessonId == l.Id));

                    if (finishedCourse)
                    {
                        var certificate = await _certificateRepository.GetOneAsync(c =>
                            c.StudentId == student1.Id &&
                            c.CourseId == lesson.CourseId);

                        if (certificate == null)
                        {
                            await _certificateRepository.AddAsync(new Certificate
                            {
                                StudentId = student1.Id,
                                CourseId = lesson.CourseId,
                                CertificateNumber = "CERT-" +
                                    Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                            });

                            await _certificateRepository.CommitAsync();
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

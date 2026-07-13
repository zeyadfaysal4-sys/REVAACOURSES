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
        private readonly IRepository<StudentProgress> _progressRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Certificate> _certificateRepository;
        public QuizController(IRepository<Quiez> quizRepository, IRepository<Student> studentRepository, UserManager<ApplicationUser> userManager, IRepository<Question> questionRepository, IRepository<StudentProgress> progressRepository, IRepository<Lesson> lessonRepository, IRepository<Certificate> certificateRepository)
        {
            _quizRepository = quizRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
            _questionRepository = questionRepository;
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
            {
                return NotFound();
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
                                        CertificateNumber = Guid.NewGuid().ToString()
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

using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.ASSISTANT_ROLE},{CD.INSTRUCTOR_ROLE}")]
    public class EnrollmentController : Controller
    {
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Course> _courseRepository;

        public EnrollmentController(
            IRepository<Enrollment> enrollmentRepository,IRepository<Student> studentRepository, IRepository<Course> courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }
            //ViewBag.Students = await _studentRepository.GetAsync(
            //    includes: [s => s.User]
            //);

            //ViewBag.Courses = await _courseRepository.GetAsync();

        public async Task<IActionResult> Index()
        {
           

            var enrollments = await _enrollmentRepository.GetAsync(includes: [s => s.Student,s=>s.Student.User,s => s.Course]);

            return View(enrollments.AsQueryable());
        }

        [HttpGet]
        public async Task<IActionResult> CheckStudent(int studentId, int courseId)
        {
            var enrollments = await _enrollmentRepository.GetAsync();

            bool isEnrolled = enrollments.Any(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId
            );

            if (isEnrolled)
            {
                TempData["Success"] = "Student is enrolled in this course";
            }
            else
            {
                TempData["Error"] = "Student is NOT enrolled in this course";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

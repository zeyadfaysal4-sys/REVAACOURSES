using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using REVAACOURSES.Documents;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CertificateController : Controller
    {
        private readonly IRepository<Certificate> _certificateRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CertificateController(IRepository<Certificate> certificateRepository, IRepository<Student> studentRepository, UserManager<ApplicationUser> userManager)
        {
            _certificateRepository = certificateRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
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
                return NotFound();
            }

            var certificates = await _certificateRepository.GetAsync(c => c.StudentId == student.Id, includes: [c => c.Course]);

            return View(certificates);
        }

        public async Task<IActionResult> Details(int id)
        {
            var certificate = await _certificateRepository.GetOneAsync(c => c.Id == id, includes: [c => c.Course, c => c.Student, c => c.Student.User]);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }
        public async Task<IActionResult> Download(int id)
        {
            var certificate = await _certificateRepository.GetOneAsync(c => c.Id == id, includes: [c => c.Course, c => c.Student, c => c.Student.User]);

            if (certificate == null)
            {
                return NotFound();
            }

            var document = new CertificateDocument(certificate);

            var pdf = document.GeneratePdf();

            return File(pdf,"application/pdf",$"{certificate.Course.Title}-Certificate.pdf");
        }
    }

}


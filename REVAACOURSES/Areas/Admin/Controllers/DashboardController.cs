using REVAACOURSES.Data;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using REVAACOURSES.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")] 
    [Area("Admin")]
    public class DashboardController : Controller
    {
        ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardVM = new DashboardVM
            {
                Course = _context.Courses.Count(),
                User = _context.Users.Count(),
                Categories = _context.Categories.Count(),
                Payment = _context.Payments.Count(),
                TotalPrice = _context.Payments.Sum(p => p.Amount)
            };

            return View(dashboardVM);
        }
    }
}

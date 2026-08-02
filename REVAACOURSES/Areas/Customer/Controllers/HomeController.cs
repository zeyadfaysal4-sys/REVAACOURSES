using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;  
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.ViewModels;
using REVAACOURSES.Data;
using Microsoft.EntityFrameworkCore;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        IRepository<Course> _CourseRepository;
        IRepository<Category> _CategoryRepository;
        IRepository<Review> _ReviewRepository;
        private readonly ApplicationDbContext _context;


        public HomeController(IRepository<Course> courseRepository, IRepository<Category> categoryRepository, IRepository<Review> reviewRepository, ApplicationDbContext context)
        {
            _CourseRepository = courseRepository;
            _CategoryRepository = categoryRepository;
            _ReviewRepository = reviewRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(FilterCourseVM filter)
        {

            if (filter.Page == 0)
            {
                filter.Page = 1;
            }

            var course = await _CourseRepository.GetAsync(includes: [c => c.Category]);

            if (filter.Title != null)
            {
                course = course.Where(c => c.Title.Contains(filter.Title));
                ViewBag.Title = filter.Title;
            }

            if (filter.CategoryId != null)
            {
                course = course.Where(c => c.CategoryId == filter.CategoryId);
                ViewBag.CategoryId = filter.CategoryId;
            }

            if (filter.Price != null)
            {
                course = course.Where(c => c.Price == filter.Price);
                ViewBag.Price = filter.Price;
            }

            if (filter.CreatedAt != null)
            {
                course = course.Where(c => c.CreatedAt == filter.CreatedAt);
                ViewBag.CreatedAt = filter.CreatedAt;
            }

            ViewBag.Categories = await _CategoryRepository.GetAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(course.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            course = course.Skip((filter.Page - 1) * 8).Take(8).ToList();


            return View(course.AsEnumerable());
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _CourseRepository.GetOneAsync(c=>c.Id == id, includes: [c => c.Category]);

            if(course == null)
            {
                return NotFound();
            }

            var relatedCourses = await _CourseRepository.GetAsync(c => c.CategoryId == course.CategoryId && c.Id != id, includes: [c => c.Category]);
            relatedCourses = relatedCourses.Skip(0).Take(3);

            var reviews = await _context.Reviews
                .Include(r => r.Student).ThenInclude(s => s.User)
                .Where(r => r.CourseId == id)
                .ToListAsync();

            reviews = reviews.Skip(0).Take(3).ToList();
            return View(new RelatedWithCourse()
            {
                Course = course,
                RelatedCourses = relatedCourses.ToList(),
                Reviews = reviews.ToList()
            });
        }
    }
}

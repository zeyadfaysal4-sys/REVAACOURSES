using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;  

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        IRepository<Course> _CourseRepository;
        IRepository<Category> _CategoryRepository;

        public HomeController(IRepository<Course> courseRepository, IRepository<Category> categoryRepository)
        {
            _CourseRepository = courseRepository;
            _CategoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(FilterCourseVM filter)
        {
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
            ViewBag.TotalPages = (int)Math.Ceiling(course.Count() / 6.0);
            ViewBag.CurrentPage = filter.Page;

            course = course.Skip((filter.Page - 1) * 6).Take(6).ToList();

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

            
            return View(new RelatedWithCourse()
            {
                Course = course,
                RelatedCourses = relatedCourses.ToList()
            });
        }
    }
}

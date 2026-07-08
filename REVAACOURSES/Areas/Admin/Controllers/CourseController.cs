using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using REVAACOURSES.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.ASSISTANT_ROLE},{CD.INSTRUCTOR_ROLE}")]
    public class CourseController : Controller
    {
        IRepository<Course> _CourseRepository;
        IRepository<Category> _CategoryRepository;

        public CourseController(IRepository<Course> courseRepository, IRepository<Category> categoryRepository)
        {
            _CourseRepository = courseRepository;
            _CategoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(FilterCourseVM filter)
        {
            var course = await _CourseRepository.GetAsync(includes: [c => c.Category]);

            if(filter.Title != null)
            {
                course = course.Where(c => c.Title.Contains(filter.Title));
                ViewBag.Title = filter.Title;
            }

            if(filter.CategoryId != null)
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

            course = course.Skip((filter.Page - 1) * 6).Take(6);

           return View(course.AsEnumerable());

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _CategoryRepository.GetAsync();
            ViewBag.Categories = categories;

            return View(new CourseVM()
            {
                Categories = categories.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Course course, IFormFile ImgFile)
        {

            if (ImgFile != null && ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                var fileName = Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CourseImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                course.Img = fileName;
            }

            course.CreatedAt = DateTime.Now;
            var SavedCourse = await _CourseRepository.AddAsync(course);
            await _CourseRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
         
        }
        public async Task<IActionResult> Update(int id)
        {
            var course = await _CourseRepository.GetOneAsync(c => c.Id == id);
            if(course == null)
            {
                return NotFound();
            }
            return View(new CourseVM()
            {
                Course = course,
                Categories = (await _CategoryRepository.GetAsync()).ToList()
            });
        }
        [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]
        [HttpPost]
        public async Task<IActionResult> Update(Course course, IFormFile ImgFile)
        {
            var oldCourse = await _CourseRepository.GetOneAsync(b => b.Id == course.Id , tracked: false);

            if (oldCourse == null)
            {
                return NotFound();
            }

            if (ImgFile != null && ImgFile.Length > 0)
            {
             
                var fileName = Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CourseImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                course.Img = fileName;

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CourseImages", oldCourse.Img);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath); 
                }
            }
            else
            {
                course.Img = oldCourse.Img;
            }

            _CourseRepository.UpdateAsync(course);
            await _CourseRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var course =await _CourseRepository.GetOneAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CourseImages", course.Img);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            _CourseRepository.DeleteAsync(course);
            await _CourseRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

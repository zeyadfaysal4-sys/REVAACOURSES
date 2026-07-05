using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE},{CD.ASSISTANT_ROLE},{CD.INSTRUCTOR_ROLE}")]
    public class CategoryController : Controller
    {
        IRepository<Category> _CategoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _CategoryRepository = categoryRepository;
        }


       
        public async Task<IActionResult> Index()
        {
            var categories = await _CategoryRepository.GetAsync();
            return View(categories.AsQueryable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error-Notification"] = "Invalid Data";
                return View(category);
            }
            await _CategoryRepository.AddAsync(category);
            await _CategoryRepository.CommitAsync();
            TempData["Success-Notification"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = $"{CD.ADMIN_ROLE} , {CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id)
        {

            var category = await _CategoryRepository.GetOneAsync(c => c.CategoryId == id);
            if (category is null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]

        public async Task<IActionResult> Update(Category category)
        {
            _CategoryRepository.UpdateAsync(category);
            await _CategoryRepository.CommitAsync();

            TempData["Success-Notification"] = "Category updated successfully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = $"{CD.ADMIN_ROLE},{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {

            var category = await _CategoryRepository.GetOneAsync(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound();
            }

            _CategoryRepository.DeleteAsync(category);
            await _CategoryRepository.CommitAsync();

            TempData["Success-Notification"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

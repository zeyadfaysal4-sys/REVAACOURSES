using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Course> _courseRepository;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Course> courseRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserId(User);
            if (user is null)
            {
                return NotFound();
            }

            var carts = await _cartRepository.GetAsync(c => c.ApplicationUserId == user, includes: [c => c.Course]);
            var TotalPrice = carts.Sum(c => c.Price * c.Count);
            ViewData["TotalPrice"] = TotalPrice;

            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int courseId, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var course = await _courseRepository.GetOneAsync(c => c.Id == courseId);
            if (course is null)
            {
                return NotFound();
            }

            var cartInDb = await _cartRepository.GetOneAsync(c => c.CourseId == courseId && c.ApplicationUserId == user.Id);

            if (cartInDb != null)
            {
                cartInDb.Count += count;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart();

            cart.ApplicationUserId = user.Id;
            cart.CourseId = courseId;
            cart.Count = count;
            cart.Price = course.Price;

            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementCount(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(c =>
                c.CourseId == courseId &&
                c.ApplicationUserId == user.Id);

            if (cartInDb == null)
                return NotFound();

            var course = await _courseRepository.GetOneAsync(c => c.Id == courseId);

            cartInDb.Count++;
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementCount(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(c =>
                c.CourseId == courseId &&
                c.ApplicationUserId == user.Id);

            if (cartInDb == null)
                return NotFound();

            if (cartInDb.Count > 1)
            {
                cartInDb.Count--;
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var cartInDb = await _cartRepository.GetOneAsync(c => c.CourseId == courseId && c.ApplicationUserId == user.Id);

            if (cartInDb == null)
            {
                return NotFound();
            }

            _cartRepository.DeleteAsync(cartInDb);
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
 
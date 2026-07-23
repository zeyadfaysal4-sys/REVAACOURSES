using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using Stripe.Checkout;

namespace REVAACOURSES.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Payment> _paymentRepository;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Course> courseRepository, IRepository<Enrollment> enrollmentRepository, IRepository<Student> studentRepository, IRepository<Payment> paymentRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserId(User);
            if (user is null)
            {
                return NotFound();
            }

            var carts = await _cartRepository.GetAsync(c => c.ApplicationUserId == user, includes: [c => c.Course]);
            var TotalPrice = carts.Sum(c => c.Price);
            ViewData["TotalPrice"] = TotalPrice;

            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int courseId)
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
                TempData["Error-Notification"] = "This course is already in your cart.";
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart
            {
                ApplicationUserId = user.Id,
                CourseId = courseId,
                Count = 1,
                Price = course.Price
            };

            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommitAsync();

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
        public async Task<IActionResult> Pay(int courseId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Cart/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Cart/Cancel",
            };

            var user = await _userManager.GetUserAsync(User);
            if (user is null) { return NotFound(); }
            var carts = await _cartRepository.GetAsync(c => c.ApplicationUserId == user.Id, includes: [p => p.Course]);
            if (carts is null) { return NotFound(); }

            foreach (var cart in carts)
            {
                var sessionLineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Course.Title,
                            Description = cart.Course.Description,
                        },
                        UnitAmount = (long)cart.Price * 100

                    },
                    Quantity = cart.Count,
                };
                options.LineItems.Add(sessionLineItemOptions);
            }

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success(string session_id)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(session_id);

                if (session.PaymentStatus != "paid")
                    return Content("Payment not paid");

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    return Content("User is null");

                var student = await _studentRepository.GetOneAsync(s => s.UserId == user.Id);

                if (student == null)
                    return Content("Student is null");

                var carts = await _cartRepository.GetAsync(
                    c => c.ApplicationUserId == user.Id,
                    includes: [c => c.Course]);

                foreach (var cart in carts)
                {
                    var exists = await _enrollmentRepository.GetOneAsync(e =>
                        e.StudentId == student.Id &&
                        e.CourseId == cart.CourseId);

                    if (exists == null)
                    {
                        var enrollment = await _enrollmentRepository.AddAsync(new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = cart.CourseId,
                            EnrollmentDate = DateTime.Now,
                            status = true
                        });
                        await _enrollmentRepository.CommitAsync();
                        var payment = new Payment
                        {
                            courseId = cart.CourseId,
                            Amount = (double)(cart.Price * (double)cart.Count),
                            PaymentDate = DateTime.Now,
                            Status = PaymentStatus.Completed,
                            TransactionId = session.PaymentIntentId
                        };
                        await _paymentRepository.AddAsync(payment);
                        await _paymentRepository.CommitAsync();
                    }

                    _cartRepository.DeleteAsync(cart);
                }

                await _enrollmentRepository.CommitAsync();
                await _cartRepository.CommitAsync();

                TempData["Success-Notification"] = "Payment completed successfully.|| Go to Page My Courses";

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }

}
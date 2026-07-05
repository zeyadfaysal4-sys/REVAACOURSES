using REVAACOURSES.Models;
using REVAACOURSES.ViewModels;
using REVAACOURSES.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using REVAACOURSES.Utiltes;

namespace REVAACOURSES.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _ApplicationUserOTP;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> otpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _ApplicationUserOTP = otpRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                Address = registerVM.Address,
                UserName = registerVM.UserName,
                PhoneNumber = registerVM.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            TempData["Success-Notification"] = "User Created Successfully ";
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(registerVM.Email, "Ecommerc Confirm Email "
                , $"<h1>Click <a href={link}> here </a> To Confirm Your Email</h1>");
            await _userManager.AddToRoleAsync(user, CD.STUDENT_ROLE);
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                TempData["Error-Notification"] = "invalid User";
                return RedirectToAction(nameof(Login));
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error-Notification"] = "Cant confirm email";
                return RedirectToAction(nameof(Login));
            }
            TempData["Success-Notification"] = "confirmed Email Successfully ";

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
             await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid User or password");
                return View(resendEmailConfirmationVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Ecommerc Confirm Email "
                , $"<h1>Click <a href={link}> here </a> To Confirm Your Email</h1>");
            TempData["Success-Notification"] = "Resend Email Confirmation Successfully ";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid User or password");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked out, please try again later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please Confirm Your Email First");
                }
                else
                {
                    ModelState.AddModelError("", "invalid User or password");
                }
                return View(loginVM);
            }
            TempData["Success-Notification"] = "Login Successfully ";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid User or password");
                return View(forgetPasswordVM);
            }

            var applicationuserotp = await _ApplicationUserOTP.GetAsync(a => a.ApplicationUserId == user.Id);
            var Count = applicationuserotp.Count(a => (DateTime.UtcNow - a.CreatedAt).TotalHours <= 24);
            if (Count >= 5)
            {
                ModelState.AddModelError("", "To Many Attempts Please Try Again Later");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOTPs = new ApplicationUserOTP(user.Id, otp);

            await _emailSender.SendEmailAsync(user.Email, "Ecommerc Forget Password"
               , $"<h1>Use This OTP <span style=\"color: red\">{otp} </span> To Reset Your Password</h1>");
            await _ApplicationUserOTP.AddAsync(applicationUserOTPs);
            await _ApplicationUserOTP.CommitAsync();
            return RedirectToAction(nameof(ValidateOTP), new { userId = user.Id });
        }
        [HttpGet]
        public IActionResult ValidateOTP(string userid)
        {
            return View(new ValidateOTPVM() { UserId = userid });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var user = await _userManager.FindByIdAsync(validateOTPVM.UserId);

            if (user is null)
            {
                ModelState.AddModelError("", "invalid User ");
                return View(validateOTPVM);
            }

            var otps = await _ApplicationUserOTP.GetAsync(a => a.ApplicationUserId == user.Id &&
            a.IsValied == true &&
            a.ValiedTo >= DateTime.UtcNow
            );

            var otp = otps.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            if (otp is null || otp.OTP != validateOTPVM.OTP)
            {
                ModelState.AddModelError("", "invalid / Expired OTP");
                return View(validateOTPVM);
            }
            otp.IsValied = false;
            await _ApplicationUserOTP.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["Token"] = token;
            return RedirectToAction(nameof(NewPassword), new { userid = user.Id });
        }
        [HttpGet]
        public IActionResult NewPassword(string userid)
        {
            var token = TempData["Token"] as string;
            if (token is null)
            {
                return RedirectToAction(nameof(Login));
            }
            return View(new NewPasswordPVM() { UserId = userid, Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordPVM newPasswordPVM)
        {
            if (newPasswordPVM.Token is null)
            {
                return RedirectToAction(nameof(Login));
            }
            var user = await _userManager.FindByIdAsync(newPasswordPVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid / Expired OTP");
                return View(newPasswordPVM);
            }
            var result = await _userManager.ResetPasswordAsync(user, newPasswordPVM.Token, newPasswordPVM.Password);
            if (result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(newPasswordPVM);
            }
            return RedirectToAction(nameof(Login));
        }
        public IActionResult AccessDenied(string userid)
        {

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}


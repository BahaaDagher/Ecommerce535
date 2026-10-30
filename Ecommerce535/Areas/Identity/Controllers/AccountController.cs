using Ecommerce535.Repositories;
using Ecommerce535.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce535.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager ;
        private readonly SignInManager<ApplicationUser> _signInManager ;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOtp> applicationUserOtpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOtpRepository = applicationUserOtpRepository;
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
                Name = registerVM.Name , 
                Address = registerVM.Address ,
                UserName = registerVM.UserName ,
                Email = registerVM.Email 
            };

            var result =  await _userManager.CreateAsync(user , registerVM.Password); 
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); 
                }
                return View(registerVM); 
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var link =  Url.Action("ConfirmEmail" , "Account" , new { area="Identity"  , id = user.Id , token} , Request.Scheme); 
            await _emailSender.SendEmailAsync(
                registerVM.Email , 
                "Ecommerce535 Confirmation Mail " ,
                $"<h1> please click <a href='{link}'> here</a> to confirm Your Mail</h1>"
                ); 
            TempData["Successful_Notification"] = "Registration Successfully"; 
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail( string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(); 
            var result =  await _userManager.ConfirmEmailAsync(user , token);
            if (!result.Succeeded)
            {
                TempData["Error_Notification"] = "Error Happen whil Confirm Email "; 
                return RedirectToAction(nameof(Login));
            }
            TempData["Successful_Notification"] = "confirm Email Successfully";
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
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail)??
                        await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);

            if(user == null)
            {
                ModelState.AddModelError("", "invalid userName or Email or Password "); 
                return View(loginVM); 
            }

            var result =  await _signInManager.PasswordSignInAsync(user , loginVM.Password , loginVM.RememberMe ,true); 
            if(!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "too many attempts please try again later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "confirm Your mail first");

                }
                else
                {
                    ModelState.AddModelError("", "invalid userName or Email or Password ");
                }
                return View(loginVM); 
            }
            TempData["Successful_Notification"] = "Login Successfully";
            return RedirectToAction("Index" , "Home" , new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "invalid userName or Email");
                return View(resendEmailConfirmationVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce535 Confirmation Mail ",
                $"<h1> please click <a href='{link}'> here</a> to confirm Your Mail</h1>"
                );
            TempData["Successful_Notification"] = "Registration Successfully";
            return RedirectToAction(nameof(Login));
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
            if(user == null)
            {
                ModelState.AddModelError("" , "invalid user");
                return View(forgetPasswordVM); 
            }

            string otp = new Random().Next(1000 , 9999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(user.Id  , otp );
            
            await _applicationUserOtpRepository.AddAsync(applicationUserOtp); 
            await _applicationUserOtpRepository.CommitAsync(); 

            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce535 Forget Password ",
                $" use this OTP {otp} to Reset your password"
                );
            TempData["Successful_Notification"] = "otp send successfully";
            return RedirectToAction(nameof(VerifyOTP) , new { userId = user.Id});
        }
        [HttpGet]
        public IActionResult VerifyOTP(string userId)
        {
            return View(new VerifyOTPVM() { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTP(VerifyOTPVM verifyOTPVM)
        {
            var user = await _userManager.FindByIdAsync(verifyOTPVM.UserId) ;
            if (user == null )
            {
                ModelState.AddModelError("", "invalid user");
                return View(verifyOTPVM);
            }
            var applicationUserOtps = await _applicationUserOtpRepository.GetAllAsync(a =>
                a.ApplicationUserId == user.Id &&
                a.IsValid && 
                a.ValidTo >= DateTime.UtcNow 
                );
            var applicationUserOtp = applicationUserOtps.OrderBy(o => o.CreatedAt).LastOrDefault();
            if(applicationUserOtp == null || applicationUserOtp.OTP != verifyOTPVM.OTP)
            {
                ModelState.AddModelError("", "invalid / Expired OTP");
                return View(verifyOTPVM);
            }
            applicationUserOtp.IsValid = false;
            await _applicationUserOtpRepository.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user); 
            return RedirectToAction(nameof(ResetPassword) , new { userId = user.Id , token });
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId , string token)
        {
            return View(new  ResetPasswordVM(){ UserId = userId  , Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if(!ModelState.IsValid)
            {
                return View(resetPasswordVM); 
            }
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "invalid user");
                return View(resetPasswordVM);
            }

            var result =  await _userManager.ResetPasswordAsync(user , resetPasswordVM.Token , resetPasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            return RedirectToAction(nameof(Login)); 
        }

    }
}

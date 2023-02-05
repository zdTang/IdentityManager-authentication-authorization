using IdentityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityManager.Controllers
{
    public class AccountController : Controller
    {
       // private readonly UserManager<IdentityUser> _userManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _userSignInManager;
        private readonly IEmailSender _emailSender;
       
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> userSignInManager,IEmailSender emailSender)
        {
            _userManager = userManager;
            _userSignInManager = userSignInManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        // Here, I don't think the returnUrl is necessary !!
        [HttpGet]
        public IActionResult Register(string returnUrl=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel); 
        }
        
        
        // Here, I don't think the returnUrl is necessary !!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model,string returnUrl=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                // create a user instance
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name };
                
                // Need UserManger to create a new USER !
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userSignInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnUrl??Url.Content("~/"));
                }
                AddErrors(result);
            }
            return View(model); 

        }
        
        /// <summary>
        /// The returnURL will be appended by the Framework when be directed here from other Action
        /// For example, When I was not login, and I tried to access an Method "Privacy"  which use [Authenticate]
        /// I will be redirected here and a returnUrl will be appended to the URL
        /// </summary>

        [HttpGet]
        public IActionResult Login(string returnUrl=null)
        {
            // here the empty viewModel is not necessary!
            ViewData["ReturnUrl"] = returnUrl;
            return View(); 
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl=null)
        {
            if (ModelState.IsValid)
            {
               
                // Login a user
                // The last parameter is to say, if the login is failed, exceed the maximum times, if we lockout the user?
                var result = await _userSignInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,lockoutOnFailure:true);
               // This approach need use IdentityUser Object
               //var result2 = await _userSignInManager.SignInAsync();
                if (result.Succeeded)
                {
                    // Be aware the difference between RedirectionToAction and Redirect
                    return returnUrl==null ? RedirectToAction("Index", "Home") : LocalRedirect(returnUrl) ;
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                //AddErrors(result.);   // SignInResult has not Errors property
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
                
                
            }
            return View(model);  

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _userSignInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        // Helper Method
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // If the user is not in DB, We should let him know and lead him to Register page
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password - Identity Manager",
                    "Please reset your password by clicking here:<a href=\"" + callbackUrl + "\">link</a>");
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);

        }

        /*The URL in the Email will trigger this Action to help user reset password*/
        [HttpGet]
        public IActionResult ResetPassword(string code=null)
        {
            return code==null?View("Error"):View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // If the user is not in DB, We should let him know and lead him to Register page
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
                AddErrors(result);
            }
            return View();

        }


        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}



using System.Security.Claims;
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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        "Please confirm your account by clicking here:<a href=\"" + callbackUrl + "\">link</a>");

                    //await _userSignInManager.SignInAsync(user, isPersistent: false);

                    //return LocalRedirect(returnUrl??Url.Content("~/"));
                    return View("TellUserConfirmEmail");
                }
                AddErrors(result);
            }
            return View(model); 

        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _userSignInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index","Home");
            }
           
            return View("Error");
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
                    return View("WrongProvidedEmail");
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

        /*==============
         Once click the "facebook".. button on the Login page
         The execution will come here.
        1, here to tell the "External login" which endpoint can be used to callback
         ===============*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl=null)
        {
            // request a redirect to the external login provider
            var redirecturl=Url.Action("ExternalLoginCallback","Account",new {returnUrl});
            var properties = _userSignInManager.ConfigureExternalAuthenticationProperties(provider, redirecturl);
            return Challenge(properties, provider);
        }



        /*==============
         Once click the "login" of the "facebook" page's  Login button
         The execution will come here.
        
        1, here to we can check if External authentication is successful.
        if success: two use cases:
         a: it is already a registered user.
         b: it is a new user
         ===============*/

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError=null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty,$"Error from external provider:{remoteError}");
                return View(nameof(Login));
            }

            var info = await _userSignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            // Sign in the user with this external login provider, if the user already has a login
            var result =
                await _userSignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,isPersistent: false);
            if (result.Succeeded)
            {
                //update any authentication tokens
                await _userSignInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnUrl);
            }
            //If the user does not have account,then we will ask the user to create an account
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email,Name=name });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,string returnUrl=null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var info = await _userSignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _userSignInManager.SignInAsync(user, isPersistent: false);
                        await _userSignInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user=await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var token = await _userManager.GetAuthenticatorKeyAsync(user);
            var model = new TwoFactorAuthenticationViewModel() { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(TwoFactorAuthenticationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user=await _userManager.GetUserAsync(User);
                var succeed = await _userManager.VerifyTwoFactorTokenAsync(user,
                    _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);
                if (succeed)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                }
                else
                {
                    ModelState.AddModelError("Verify","Your two factor auth code could not be validated.");
                    return View(model);
                }
            }

            return RedirectToAction("AuthenticatorConfirmation");
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

        }

    }
}



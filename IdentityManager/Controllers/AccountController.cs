using System.Threading.Tasks;
using IdentityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace IdentityManager.Controllers
{
    public class AccountController : Controller
    {
       // private readonly UserManager<IdentityUser> _userManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _userSignInManager;
       
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> userSignInManager)
        {
            _userManager = userManager;
            _userSignInManager = userSignInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel); 
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // create a user instance
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name };
                
                // Need UserManger to create a new USER !
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userSignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
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
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string returnUrl=null)
        {
            // here the empty viewModel is not necessary!
            ViewData["ReturnUrl"] = returnUrl;
            return View(); 
        }
        
        /// <summary>
        /// Be aware of the parameters
        /// The model come from Form body
        /// The returnUrl comes from queryString
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl=null)
        {
            if (ModelState.IsValid)
            {
               
                // Login a user
                // The last parameter is to say, if the login is failed, if we lockout the user?
                var result = await _userSignInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);
               // This approach need use IdentityUser Object
               //var result2 = await _userSignInManager.SignInAsync();
                if (result.Succeeded)
                {
                    // Be aware the difference between RedirectionToAction and Redirect
                    return returnUrl==null ? RedirectToAction("Index", "Home") : Redirect(returnUrl) ;
                }
                //AddErrors(result.);   // SignInResult has not Errors property
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
                
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
        
    }
}

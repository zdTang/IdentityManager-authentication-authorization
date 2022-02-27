using System.Threading.Tasks;
using IdentityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers
{
    public class AccountController : Controller
    {
       // private readonly UserManager<IdentityUser> _userManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _userSignInManager;
       
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> userSignInManager)
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
                
                // check if the usrName is available, if Yes then create 
                
                // if No, return a  New Error message !
                ModelState.AddModelError("Occupied","The userName has been occupied !");
                return View(model); 
            }
           else
           {
               return View(model); 
           }
        }
    }
}

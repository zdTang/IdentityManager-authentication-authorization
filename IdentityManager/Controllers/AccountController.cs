﻿using IdentityManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Register()
        //{
        //    var registerViewModel = new RegisterViewModel();
        //    return View(registerViewModel); 
        //}

        public IActionResult Register()
        {
            //var registerViewModel = new RegisterViewModel();
            return View();
        }
    }
}

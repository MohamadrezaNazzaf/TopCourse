using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TopCourse.Core.Services.Interfaces;
using TopCourse.Core.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TopCourse.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]

    public class HomeController : Controller
    {
        IUserService _userService;
        ICourseService _courseService;
        public HomeController(IUserService userService, ICourseService courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }
        
        public IActionResult Index()
        {
            return View(_userService.GetUserInformation(User.Identity.Name));
        }

        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userService.GetDataForEditProfileUser(User.Identity.Name));
        }

        [Route("UserPanel/EditProfile")]
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            var resultEdit = _userService.EditProfile(User.Identity.Name, profile);
            if (resultEdit == true)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Redirect("/Login?EditProfile=true");
            
        }

        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Route("UserPanel/ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            string currentUserName = User.Identity.Name;
            if (!ModelState.IsValid)
                return View(changePassword);
            if(!_userService.CompareOldPassword(changePassword.OldPassword,currentUserName))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی صحیح نمی باشد.");
                return View(changePassword);
            }
            _userService.ChangeUserPassword(currentUserName,changePassword.Password);
            ViewBag.IsSuccess = true;
            return View();
        }

        [Route("UserPanel/MyCourses")]
        public IActionResult MyCourses()
        {
            return View(_courseService.UserCourses(User.Identity.Name));
        }
    }
}
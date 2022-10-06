using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.DTOs;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Pages.Admin.Users
{
    [PermissionChecker(7)]
    public class RecoveryUserModel : PageModel
    {
        private IUserService _userService;
        public RecoveryUserModel(IUserService userService)
        {
            _userService = userService;
        }
        public InformationUserViewModel InformationUserViewModel { get; set; }
        public void OnGet(int id)
        {
            ViewData["UserId"] = id;
            InformationUserViewModel = _userService.GetUserInformationForRecovery(id);
        }
        public IActionResult OnPost(int userId)
        {
            _userService.RecoveryUser(userId);
            return RedirectToPage("IndexUser");
        }
    }
}
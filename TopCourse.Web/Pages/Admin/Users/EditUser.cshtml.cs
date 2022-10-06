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
    [PermissionChecker(4)]
    public class EditUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        public void OnGet(int id)
        {
            EditUserViewModel = _userService.GetUserForShowInEditMode(id);
            ViewData["Roles"] = _permissionService.GetRole();
        }
        public IActionResult OnPost(List<int> SelectedRoles,bool isActive)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _userService.EditUserFromAdmin(EditUserViewModel,isActive);

            //Edit Roles
            _permissionService.EditRolesToUser(SelectedRoles,EditUserViewModel.userId);
            return RedirectToPage("IndexUser");
        }
    }
}
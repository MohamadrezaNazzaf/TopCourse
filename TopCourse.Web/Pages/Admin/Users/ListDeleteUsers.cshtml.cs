﻿using System;
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
    [PermissionChecker(6)]
    public class ListDeleteUsersModel : PageModel
    {
        private IUserService _userService;
        public ListDeleteUsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public UserForAdminViewModel UserForAdminViewModel { get; set; }
        public void OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            UserForAdminViewModel = _userService.GetDeleteUser(pageId, filterEmail, filterUserName);
            
        }
    }
}
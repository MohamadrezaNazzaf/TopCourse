﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.DTOs;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.User;

namespace TopCourse.Web.Pages.Admin.Roles
{
    [PermissionChecker(8)]
    public class IndexModel : PageModel
    {
        private IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public List<Role> RolesList { get; set; }
        public void OnGet()
        {
            RolesList = _permissionService.GetRole();
        }
    }
}
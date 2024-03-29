﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Web.Pages.Admin.CourseGroups
{
    public class EditGroupModel : PageModel
    {
        private ICourseService _courseService;
        public EditGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [BindProperty] // injouri be safhe motasel mishe
        public CourseGroup CourseGroup { get; set; }

        public void OnGet(int id)
        {
            CourseGroup = _courseService.GetGroupById(id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _courseService.UpdateGroup(CourseGroup);
            return RedirectToPage("IndexGroup");
        }
    }
}
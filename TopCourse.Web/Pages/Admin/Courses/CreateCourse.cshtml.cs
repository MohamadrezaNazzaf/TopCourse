﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Web.Pages.Admin.Courses
{
    [PermissionChecker(1007)]
    public class CreateCourseModel : PageModel
    {
        private ICourseService _courseService;
        public CreateCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet()
        {
            var groups = _courseService.GetGroupForManageCourse();
            ViewData["Groups"] = new SelectList(groups,"Value","Text");

            var subGroups = _courseService.GetSubGroupForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text");

            var teachers = _courseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

            var levels = _courseService.GetLevels();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text");

            var statues = _courseService.GetStatues();
            ViewData["Statues"] = new SelectList(statues, "Value", "Text");

        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid)
                return Page();

            _courseService.AddCourse(Course,imgCourseUp,demoUp);
            System.Threading.Thread.Sleep(2000);
            return RedirectToPage("IndexCourse");
        }
        
    }
}
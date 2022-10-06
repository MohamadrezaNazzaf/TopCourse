using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Web.Pages.Admin.CourseGroups
{
    public class CreateGroupModel : PageModel
    {
        private ICourseService _courseService;
        public CreateGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [BindProperty] // injouri be safhe motasel mishe
        public CourseGroup CourseGroup { get; set; }

        public void OnGet(int? id)
        {
            CourseGroup = new CourseGroup()
            {
                ParentId = id 
            };
            if(id!=null)
            {
                var group = _courseService.GetGroupById(id.Value);
                ViewData["groupTitle"] = group.GroupTitle;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _courseService.AddGroup(CourseGroup);
            return RedirectToPage("IndexGroup");
        }
    }
}
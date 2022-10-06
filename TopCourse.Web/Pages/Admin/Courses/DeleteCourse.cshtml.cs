using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Web.Pages.Admin.Courses
{
    [PermissionChecker(2007)]
    public class DeleteCourseModel : PageModel
    {
        private ICourseService _courseService;
        public DeleteCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }
        static int currentPage = 0;
        public void OnGet(int id, int pageId)
        {
            Course = _courseService.GetCourseById(id);
            ViewData["pageId"] = pageId;
            currentPage = pageId;
        }

        public IActionResult OnPost(int courseId)
        {
            _courseService.DeleteCourse(courseId);
            return RedirectToPage("IndexCourse",new {pageId = currentPage});
        }
    }
}
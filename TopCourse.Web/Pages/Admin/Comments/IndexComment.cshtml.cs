using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Web.Pages.Admin.Comments
{
    public class IndexModel : PageModel
    {
        private ICourseService _courseService;
        public IndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public List<CourseComment> CourseComments { get; set; }
        public void OnGet(int id)
        {
            CourseComments = _courseService.GetCommentsByCouseId(id);
            ViewData["courseName"] = _courseService.GetCourseNameById(id);
        }
    }
}
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
    public class DeleteCommentModel : PageModel
    {
        private ICourseService _courseService;
        public DeleteCommentModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [BindProperty]
        public CourseComment CourseComment { get; set; }

        public void OnGet(int id)
        {
            CourseComment = _courseService.GetCommnetById(id);
        }

        public IActionResult OnPost(int commentId, int courseId)
        {
            _courseService.DeleteComment(commentId);
            return Redirect("/Admin/Comments/IndexComment/" + courseId);
        }
    }
}
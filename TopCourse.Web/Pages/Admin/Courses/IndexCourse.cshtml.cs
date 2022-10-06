
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.DTOs.Course;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.Core.Utilities.Paging;

namespace TopCourse.Web.Pages.Admin.Courses
{
    [PermissionChecker(1006)]
    public class IndexModel : PageModel
    {
        private ICourseService _courseService;
        private IPermissionService _permissionService;
        public IndexModel(ICourseService courseService, IPermissionService permissionService)
        {
            _courseService = courseService;
            _permissionService = permissionService;
        }

        public ShowCourseViewModel ListCourse { get; set; }

        public void OnGet(int pageId = 1, string filterCourseTitle = "", string filterTeacherName = "")
        {
            var userRole = _permissionService.GetUserRoleById(User.Identity.Name);
            if(userRole.FirstOrDefault() == 1)
            {
                ListCourse = _courseService.GetCoursesForAdmin(pageId, filterCourseTitle, filterTeacherName);
            }
            else if(userRole.FirstOrDefault() == 2)
            { 
            ListCourse = _courseService.GetCoursesForTeacher(pageId, filterCourseTitle, filterTeacherName, User.Identity.Name);
            }

            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Courses/IndexCourse?pageId=:");
            ListCourse.PagingInfo = new PagingInfo()
            {
                CurrentPage = pageId,
                ItemPerPage = CountShowItemsInPages.Count,
                TotalItems = _courseService.CourseCount(),
                UrlParam = param.ToString()
            };
        }
    }
}
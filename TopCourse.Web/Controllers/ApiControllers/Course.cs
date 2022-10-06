using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TopCourse.Core.DTOs.Course;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Context;

namespace TopCourse.Web.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Course : ControllerBase
    {
        private TopCourseContext _context;
        private ICourseService _courseService;
        public Course(TopCourseContext context, ICourseService courseService)
        {
            _context = context; 
            _courseService = courseService;
        }
        [HttpGet("CourseName/{courseName}")]
        public ActionResult<IEnumerable<ShowCourseFilterViewModel>> GetCourseByCourseName([FromRoute] string courseName)
        {
            return _courseService.GetCourseByCourseName(courseName);
        }

        [HttpGet("TeacherName/{teacherName}")]
        public ActionResult<IEnumerable<ShowCourseFilterViewModel>> GetCourseByTeacherName([FromRoute] string teacherName)
        {
            return _courseService.GetCourseByTeacherName(teacherName);
        }
    }
}

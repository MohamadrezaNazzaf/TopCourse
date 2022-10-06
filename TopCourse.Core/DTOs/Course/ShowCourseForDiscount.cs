using System;
using System.Collections.Generic;
using System.Text;

namespace TopCourse.Core.DTOs.Course
{
    public class ShowCourseForDiscount
    {
        public int CourseId { get; set; }
        public string ImageName { get; set; }
        public string Title { get; set; }
        public string TeacherName { get; set; }
        public string StatusTitle { get; set; }

    }
}

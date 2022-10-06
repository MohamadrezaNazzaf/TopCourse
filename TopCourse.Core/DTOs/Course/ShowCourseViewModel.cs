using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.Core.Utilities.Paging;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Core.DTOs.Course
{
    public class ShowCourseViewModel
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public List<TopCourse.DataLayer.Entities.Course.Course> Courses { get; set; }
        public PagingInfo PagingInfo { get; set; }



    }
}

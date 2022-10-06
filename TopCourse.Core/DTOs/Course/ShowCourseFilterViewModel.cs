using System;
using System.Collections.Generic;
using System.Text;

namespace TopCourse.Core.DTOs.Course
{
    public class ShowCourseFilterViewModel
    {
        public int CourseId { get; set; }
        public string CourseImageName { get; set; }
        public string CourseTitle { get; set; }
        public string TacherName { get; set; }
        public int EpisodeCount { get; set; }
        public string CourseStatuse { get; set; }

    }
}

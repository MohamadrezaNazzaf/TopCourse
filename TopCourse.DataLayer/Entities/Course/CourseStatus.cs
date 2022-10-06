using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopCourse.DataLayer.Entities.Course
{
    public class CourseStatus
    {
        [Key]
        public int StatusId { get; set; }

        [MaxLength(150)]
        [Required]
        public string StatusTitle { get; set; }

        #region Relation
        public List<Course> Courses { get; set; }
        #endregion
    }
}

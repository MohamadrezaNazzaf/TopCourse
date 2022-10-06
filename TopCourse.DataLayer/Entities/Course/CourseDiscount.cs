using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using TopCourse.DataLayer.Entities.Order;

namespace TopCourse.DataLayer.Entities.Course
{
    public class CourseDiscount
    {
        [Key]
        public int CD_Id { get; set; }     
        public int CourseId { get; set; }
        public int DiscountId { get; set; }

        #region Relation
        public Course Course { get; set; }
        public Discount Discount { get; set; }
        #endregion
    }
}

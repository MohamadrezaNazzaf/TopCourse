﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TopCourse.DataLayer.Entities.Course
{

    public class Course
    {
        [Key]
        public int CourseId { get; set; }  

        [Required]
        public int GroupId { get; set; }

        public int? SubGroup { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int LevelId { get; set; }

        [Display(Name = "عنوان دوره")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(450, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CourseTitle { get; set; }

        [Display(Name = "شرح دوره")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string CourseDescription { get; set; }

        [Display(Name = "قیمت دوره")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CoursePrice { get; set; }

        [MaxLength(600)]
        public string Tags { get; set; }

        [MaxLength(50)]
        public string CourseImageName { get; set; }

        [MaxLength(100)]
        public string DemoFileName { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }


        #region Relations

        [ForeignKey("TeacherId")]
        public User.User User { get; set; }

        [ForeignKey("GroupId")]
        public CourseGroup CourseGroup { get; set; }

        [ForeignKey("SubGroup")]
        public CourseGroup Group { get; set; }

        public CourseStatus CourseStatus { get; set; }

        public CourseLevel CourseLevel { get; set; }

        public List<CourseEpisode> CourseEpisodes { get; set; } // Yek Course Mitavanad Motealegh Be n Ta CourseEpisode Bashad.
        public List<Order.OrderDetail> OrderDetails { get; set; } // Yek Course Mitavanad Motealegh Be n Ta OrderDetail Bashad.
        public List<UserCourse> UserCourses { get; set; } // Yek Course Mitavanad Motealegh Be n Ta UserCourse Bashad.
        public List<CourseComment> CourseComments { get; set; } // har Course Mitavanad n Ta comment dashte Bashad.
        public List<CourseDiscount> CourseDiscounts { get; set; }
        public List<CourseVote> CourseVotes { get; set; }
        #endregion
    }
}

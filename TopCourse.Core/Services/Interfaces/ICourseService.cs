using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.Core.DTOs.Course;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Group
        List<CourseGroup> GetAllGroup();
        List<SelectListItem> GetGroupForManageCourse();
        List<SelectListItem> GetSubGroupForManageCourse(int groupId);
        List<SelectListItem> GetTeachers();
        List<SelectListItem> GetLevels();
        List<SelectListItem> GetStatues();
        void AddGroup(CourseGroup group);
        void UpdateGroup(CourseGroup group);
        CourseGroup GetGroupById(int groupId);
        void DeleteGroup(int groupId);
        #endregion

        #region Course
        ShowCourseViewModel GetCoursesForAdmin(int pageId = 1, string filterCourseTitle = "", string filterTeacherName = "");
        ShowCourseViewModel GetCoursesForTeacher(int pageId = 1, string filterCourseTitle = "", string filterTeacherName = "", string teacherName = "");
        List<ShowCourseFilterViewModel> GetCourseByCourseName(string courseName);
        List<ShowCourseFilterViewModel> GetCourseByTeacherName(string teacherName);
        int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Course GetCourseById(int courseId);
        string GetCourseNameById(int courseId);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        void DeleteCourse(int courseId);
        Tuple<List<ShowCourseListItemViewModel>,int> GetCourse(int pageId=1, string filter="", string getType="all", 
            string orderByType="date", int startPrice=0, int endPrice=0, List<int> selectedGroups=null, int take = 0);
        Course GetCourseForShow(int courseId);
        List<ShowCourseListItemViewModel> GetPopularCourse();
        ShowCourseViewModel UserCourses(string userName);
        List<ShowCourseForDiscount> GetAllCoursesForDiscount();
        int CourseCount();
        bool IsFree(int courseId);

        #endregion

        #region Episode
        List<CourseEpisode> GetListEpisodeCourse(int courseId);
        CourseEpisode GetEpisodeById(int episodeId);
        string GetEpisodeNameById(int episodeId);
        bool CheckExistFile(string fileName);
        int AddEpisode(CourseEpisode episode, IFormFile episodeFile);
        void EditEpisode(CourseEpisode episode, IFormFile episodeFile);
        void DeleteEpisode(int episodeId);
        #endregion

        #region Comments
        void AddComment(CourseComment comment);
        Tuple<List<CourseComment>,int> GetCourseComments(int courseId, int pageId=1);
        List<CourseComment> GetCommentsByCouseId(int courseId);
        CourseComment GetCommnetById(int commentId);
        void DeleteComment(int commentId);

        #endregion
        #region Course Vote
        void AddVote(int userId, int courseId, bool vote);
        Tuple<int, int> GetCourseVotes(int courseId);
        #endregion
    }
}

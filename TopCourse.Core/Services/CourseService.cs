using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TopCourse.Core.Convertors;
using TopCourse.Core.DTOs.Course;
using TopCourse.Core.Generator;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.Core.Utilities.Paging;
using TopCourse.DataLayer.Context;
using TopCourse.DataLayer.Entities.Course;

namespace TopCourse.Core.Services
{
    public class CourseService : ICourseService
    {
        private TopCourseContext _context;
        private IUserService _userService;
        private CRUD<Course> _CRUD;

        public CourseService(TopCourseContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
            _CRUD = new CRUD<Course>(_context);

        }

        public List<CourseGroup> GetAllGroup()
        {
            return _context.CourseGroups.Include(c => c.CourseGroups).Where(c => !c.IsDelete).ToList();
        }

        public List<SelectListItem> GetGroupForManageCourse()
        {
            return _context.CourseGroups.Where(g => g.ParentId == null)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }


        public List<SelectListItem> GetSubGroupForManageCourse(int groupId)
        {
            return _context.CourseGroups.Where(g => g.ParentId == groupId)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetTeachers()
        {
            return _context.UserRoles.Where(r => r.RoleId == 2).Include(r => r.User)
                .Select(u => new SelectListItem()
                {
                    Value = u.UserId.ToString(),
                    Text = u.User.UserName
                }).ToList();
        }

        public List<SelectListItem> GetLevels()
        {
            return _context.CourseLevels.Select(l => new SelectListItem()
            {
                Value = l.LevelId.ToString(),
                Text = l.LevelTitle
            }).ToList();
        }

        public List<SelectListItem> GetStatues()
        {
            return _context.CourseStatuses.Select(s => new SelectListItem()
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusTitle
            }).ToList();
        }

        public int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";

            //TODO Check Image
            if (imgCourse != null && imgCourse.IsImage())
            {
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                imgResizer.Image_resize(imagePath, thumbPath, 150);
            }


            if (courseDemo != null)
            {
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            _context.Add(course); //خودش از موجودیت میفهمه روی کدوم انتیتی اد کنه
            _context.SaveChanges();

            return course.CourseId;

        }

        public ShowCourseViewModel GetCoursesForAdmin(int pageId = 1, string filterCourseTitle = "", string filterTeacherName = "")
        {
            IQueryable<Course> result = _context.Courses.Include(c => c.User).Include(c => c.CourseEpisodes).Include(c => c.CourseStatus);


            int take = CountShowItemsInPages.Count;
            int skip = (pageId - 1) * take;
            int pageCount = 0;
            for (int i = 1; i <= _context.Courses.Count(); i += take)
            {
                pageCount += 1;
            }

            if (!string.IsNullOrEmpty(filterCourseTitle))
            {
                result = result.Where(c => c.CourseTitle.Contains(filterCourseTitle));
            }
            if (!string.IsNullOrEmpty(filterTeacherName))
            {
                result = result.Where(c => c.User.UserName.Contains(filterTeacherName));
            }

            ShowCourseViewModel course = new ShowCourseViewModel();

            course.CurrentPage = pageId;
            course.PageCount = pageCount;
            course.Courses = result.OrderBy(r => r.CreateDate).Skip(skip).Take(take).ToList();

            return course;
        }

        public Course GetCourseById(int courseId)
        {
            //var cacheCourse = _cache.Get<Course>(courseId);
            //if (cacheCourse != null)
            //{
            //    return cacheCourse;
            //}
            //else
            //{
            //    var course = _CRUD.GetById(courseId);
            //    var cacheOption = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
            //    _cache.Set(courseId, course, cacheOption);
            //    return course;
            //}


            //return _CRUD.GetById(courseId);  //Generic
            return _context.Courses.Find(courseId);
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.UpdateDate = DateTime.Now;

            if (imgCourse != null && imgCourse.IsImage())
            {
                if (course.CourseImageName != "no-photo.jpg")
                {
                    string deleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);
                    if (File.Exists(deleteImagePath))
                    {
                        File.Delete(deleteImagePath);
                    }

                    string deletethumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                    if (File.Exists(deletethumbPath))
                    {
                        File.Delete(deletethumbPath);
                    }
                }
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                imgResizer.Image_resize(imagePath, thumbPath, 150);
            }


            if (courseDemo != null)
            {
                if (course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                    if (File.Exists(deleteDemoPath))
                    {
                        File.Delete(deleteDemoPath);
                    }
                }
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public int AddEpisode(CourseEpisode episode, IFormFile episodeFile)
        {
            episode.EpisodeFileName = episodeFile.FileName;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", episode.EpisodeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                episodeFile.CopyTo(stream);
            }

            _context.CourseEpisodes.Add(episode);
            _context.SaveChanges();
            return episode.EpisodeId;
        }

        public bool CheckExistFile(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", fileName);
            return File.Exists(path);
        }

        public List<CourseEpisode> GetListEpisodeCourse(int courseId)
        {
            return _context.CourseEpisodes.Where(c => c.CourseId == courseId).ToList();
        }

        public string GetCourseNameById(int courseId)
        {
            return _context.Courses.First(c => c.CourseId == courseId).CourseTitle;
        }

        public CourseEpisode GetEpisodeById(int courseId)
        {
            return _context.CourseEpisodes.Find(courseId);
        }

        public string GetEpisodeNameById(int episodeId)
        {
            return _context.CourseEpisodes.First(e => e.EpisodeId == episodeId).EpisodeTitle;
        }

        public void EditEpisode(CourseEpisode episode, IFormFile episodeFile)
        {
            if (episodeFile != null)
            {
                string deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", episode.EpisodeFileName);
                File.Delete(deleteFilePath);

                episode.EpisodeFileName = episodeFile.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", episode.EpisodeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    episodeFile.CopyTo(stream);
                }
            }
            _context.CourseEpisodes.Update(episode);
            _context.SaveChanges();
        }

        public void DeleteCourse(int courseId)
        {
            var listEpisode = GetListEpisodeCourse(courseId);
            foreach (var item in listEpisode)
            {
                var episode = GetEpisodeById(item.EpisodeId);
                string deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", episode.EpisodeFileName);
                File.Delete(deleteFilePath);
            }
            _context.Entry(GetCourseById(courseId)).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public void DeleteEpisode(int episodeId)
        {
            var episode = GetEpisodeById(episodeId);
            string deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/courseFiles", episode.EpisodeFileName);
            File.Delete(deleteFilePath);

            _context.Entry(episode).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public Tuple<List<ShowCourseListItemViewModel>, int> GetCourse(int pageId = 1, string filter = "", string getType = "all", string orderByType = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            if (take == 0)
                take = 8;

            IQueryable<Course> result = _context.Courses; // in query Lazy hast yani ta zamani ke .ToList() ya return nadare ejra nemishe, pas hame course ha ra vakeshi nakardim.
            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(c => c.CourseTitle.Contains(filter) || c.Tags.Contains(filter));
            }

            switch (getType)
            {
                case "all":
                    break;
                case "buy":
                    {
                        result = result.Where(c => c.CoursePrice != 0);
                        break;
                    }
                case "free":
                    {
                        result = result.Where(c => c.CoursePrice == 0);
                        break;
                    }
            }

            switch (orderByType)
            {
                case "data":
                    {
                        result = result.OrderByDescending(c => c.CreateDate);
                        break;
                    }
                case "updatedata":
                    {
                        result = result.OrderByDescending(c => c.UpdateDate);
                        break;
                    }
            }

            if (startPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > startPrice);
            }
            if (endPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < endPrice);
            }

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (int groupId in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == groupId || c.SubGroup == groupId);
                }
            }

            int skip = (pageId - 1) * take;

            int pageCount = result.Include(c => c.CourseEpisodes).Select(c => new ShowCourseListItemViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                Price = c.CoursePrice,
                Title = c.CourseTitle,
                TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            }).Count() / take;

            var query = result.Include(c => c.CourseEpisodes).Select(c => new ShowCourseListItemViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                Price = c.CoursePrice,
                Title = c.CourseTitle,
                TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            }).Skip(skip).Take(take).ToList();

            return Tuple.Create(query, pageCount);
        }

        public Course GetCourseForShow(int courseId)
        {
            return _context.Courses.Include(c => c.CourseEpisodes)
                .Include(c => c.CourseStatus).Include(c => c.CourseLevel)
                .Include(c => c.User).Include(c => c.UserCourses).FirstOrDefault(c => c.CourseId == courseId);
        }

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            _context.SaveChanges();
        }

        public Tuple<List<CourseComment>, int> GetCourseComments(int courseId, int pageId)
        {
            int take = 5;
            int skip = (pageId - 1) * take;
            int pageCount = _context.CourseComments.Where(c => !c.IsDelete && c.CourseId == courseId).Count() / take;

            if ((pageCount % 2) != 0)
            {
                pageCount += 1;
            }

            return Tuple.Create(
                _context.CourseComments.Include(c => c.User).Where(c => !c.IsDelete && c.CourseId == courseId).Skip(skip).Take(take)
                    .OrderByDescending(c => c.CreateDate).ToList(), pageCount);
        }

        public List<ShowCourseListItemViewModel> GetPopularCourse()
        {
            return _context.Courses.Include(c => c.OrderDetails)
                .Where(c => c.OrderDetails.Any())
                .OrderByDescending(d => d.OrderDetails.Count)
                .Take(8)
                .Select(c => new ShowCourseListItemViewModel()
                {
                    CourseId = c.CourseId,
                    ImageName = c.CourseImageName,
                    Price = c.CoursePrice,
                    Title = c.CourseTitle,
                    TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
                })
                .ToList();
        }

        public void AddGroup(CourseGroup group)
        {
            _context.CourseGroups.Add(group);
            _context.SaveChanges();
        }

        public void UpdateGroup(CourseGroup group)
        {
            _context.CourseGroups.Update(group);
            _context.SaveChanges();
        }

        public CourseGroup GetGroupById(int groupId)
        {
            return _context.CourseGroups.Find(groupId);
        }

        public void DeleteGroup(int groupId)
        {
            var group = _context.CourseGroups.Find(groupId);
            group.IsDelete = true;
            UpdateGroup(group);
        }

        public List<CourseComment> GetCommentsByCouseId(int courseId)
        {
            return _context.CourseComments.Include(c => c.User).Where(c => c.CourseId == courseId && !c.IsDelete).ToList();
        }

        public CourseComment GetCommnetById(int commentId)
        {
            return _context.CourseComments.Include(c => c.User).FirstOrDefault(c => c.CommentId == commentId && !c.IsDelete);
        }

        public void DeleteComment(int commentId)
        {
            var comment = _context.CourseComments.Find(commentId);
            comment.IsDelete = true;
            _context.CourseComments.Update(comment);
            _context.SaveChanges();
        }

        public ShowCourseViewModel GetCoursesForTeacher(int pageId = 1, string filterCourseTitle = "", string filterTeacherName = "", string teacherName = "")
        {
            var teacherId = _userService.GetUserIdByUserName(teacherName);
            IQueryable<Course> result = _context.Courses.Where(c => c.TeacherId == teacherId);


            int take = 8;
            int skip = (pageId - 1) * take;
            int pageCount = 0;
            for (int i = 1; i <= _context.Courses.Count(); i += take)
            {
                pageCount += 1;
            }

            if (!string.IsNullOrEmpty(filterCourseTitle))
            {
                result = result.Where(c => c.CourseTitle.Contains(filterCourseTitle));
            }
            if (!string.IsNullOrEmpty(filterTeacherName))
            {
                result = result.Where(c => c.User.UserName.Contains(filterTeacherName));
            }

            ShowCourseViewModel course = new ShowCourseViewModel();

            course.CurrentPage = pageId;
            course.PageCount = result.Count() / take;
            course.Courses = result.OrderBy(r => r.CreateDate).Skip(skip).Take(take).ToList();

            return course;
        }

        public ShowCourseViewModel UserCourses(string userName)
        {

            IQueryable<Course> result = _context.Courses.Include(c => c.User).Include(c => c.CourseEpisodes).Include(c => c.CourseStatus);

            int userId = _userService.GetUserIdByUserName(userName);
            var userCourse = _context.UserCourses.Where(c => c.UserId == userId).Select(c => c.CourseId);

            // var course = _context.Courses.Where(c => userCourse.Contains(c.CourseId)).Select(c => new ShowCourseForAdminViewModel()
            //{
            //    CreateDate = c.CreateDate,
            //    CourseId = c.CourseId,
            //    ImageName = c.CourseImageName,
            //    Title = c.CourseTitle,
            //    EpisodeCount = c.CourseEpisodes.Count,
            //    StatusTitle = c.CourseStatus.StatusTitle,
            //    TeacherName = c.User.UserName

            //}).ToList();

            ShowCourseViewModel course = new ShowCourseViewModel();
            course.Courses = result.Where(c => userCourse.Contains(c.CourseId)).ToList();
            return course;


        }

        public List<ShowCourseForDiscount> GetAllCoursesForDiscount()
        {
            var course = _context.Courses.Select(c => new ShowCourseForDiscount()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                TeacherName = c.User.UserName,
                Title = c.CourseTitle,
                StatusTitle = c.CourseStatus.StatusTitle

            }).ToList();

            return course;
        }

        public List<ShowCourseFilterViewModel> GetCourseByCourseName(string courseName)
        {
            var course = _context.Courses.Include(c => c.User).Include(c => c.CourseEpisodes).Include(c => c.CourseStatus)
                .Where(c => c.CourseTitle.Contains(courseName)).Select(c => new ShowCourseFilterViewModel()
                {
                    CourseId = c.CourseId,
                    CourseImageName = c.CourseImageName,
                    CourseTitle = c.CourseTitle,
                    TacherName = c.User.UserName,
                    EpisodeCount = c.CourseEpisodes.Count(),
                    CourseStatuse = c.CourseStatus.StatusTitle
                }).ToList();

            return course;
        }

        public List<ShowCourseFilterViewModel> GetCourseByTeacherName(string teacherName)
        {
            var course = _context.Courses.Include(c => c.User).Include(c => c.CourseEpisodes).Include(c => c.CourseStatus)
                .Where(c => c.User.UserName.Contains(teacherName)).Select(c => new ShowCourseFilterViewModel()
                {
                    CourseId = c.CourseId,
                    CourseImageName = c.CourseImageName,
                    CourseTitle = c.CourseTitle,
                    TacherName = c.User.UserName,
                    EpisodeCount = c.CourseEpisodes.Count(),
                    CourseStatuse = c.CourseStatus.StatusTitle
                }).ToList();

            return course;
        }

        public int CourseCount()
        {
            return _context.Courses.Count();
        }

        public void AddVote(int userId, int courseId, bool vote)
        {
            var userVote = _context.CourseVotes.FirstOrDefault(u=>u.UserId == userId && u.CourseId == courseId);
            if(userVote != null)
            {
                userVote.Vote = vote;
            }
            else
            {
                userVote = new CourseVote()
                {
                    UserId = userId,
                    CourseId = courseId,
                    Vote = vote
                };
                _context.CourseVotes.Add(userVote);
            }
            _context.SaveChanges();
        }

        public Tuple<int, int> GetCourseVotes(int courseId)
        {
            var votes = _context.CourseVotes.Where(v => v.CourseId == courseId).Select(v => v.Vote).ToList();
            return Tuple.Create(votes.Count(v=>v), votes.Count(v=>!v)); // return number true votes and number false votes.
        }

        public bool IsFree(int courseId)
        {
            int coursePrice = _context.Courses.FirstOrDefault(c => c.CourseId == courseId).CoursePrice;

            if (coursePrice == 0)
                return true;

            return false;
                
        }
    }
}

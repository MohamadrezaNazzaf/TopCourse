using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.DTOs;
using TopCourse.Core.Security;
using TopCourse.Core.Services.Interfaces;
using TopCourse.Core.Utilities.Paging;

namespace TopCourse.Web.Pages.Admin.Users
{
    [PermissionChecker(2)]
    public class IndexModel : PageModel
    {
        private IUserService _userService;
        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public UserForAdminViewModel UserForAdminViewModel { get; set; }
        public void OnGet(int pageId=1, string filterUserName="", string filterEmail="")
        {

            UserForAdminViewModel = _userService.GetUser(pageId,filterEmail,filterUserName);

            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Users/IndexUser?pageId=:");
            var userCount = _userService.UserCount();

            UserForAdminViewModel.PagingInfo = new PagingInfo()
            {
                CurrentPage = pageId,
                ItemPerPage = CountShowItemsInPages.Count,
                TotalItems = userCount,
                UrlParam = param.ToString()
            };
        }


    }
}
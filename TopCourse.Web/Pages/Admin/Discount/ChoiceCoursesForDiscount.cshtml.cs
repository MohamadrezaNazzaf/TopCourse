using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Pages.Admin.Discount
{
    public class ChoiceCoursesForDiscountModel : PageModel
    {
        private IOrderService _orderService;
        private ICourseService _courseService;
        public ChoiceCoursesForDiscountModel(IOrderService orderService, ICourseService courseService)
        {
            _orderService = orderService;
            _courseService = courseService;
        }

        static int discountId;
        public void OnGet(int id)
        {
            discountId = id;
            ViewData["CodeName"] = _orderService.GetDiscountCodeById(id);
            ViewData["Courses"] = _courseService.GetAllCoursesForDiscount();
        }
        public IActionResult OnPost(List<int> selectedCourse)
        {

            _orderService.AddCourseDiscount(selectedCourse, discountId);

            return Redirect("/Admin/Discount/IndexDiscount");
        }
    }
}
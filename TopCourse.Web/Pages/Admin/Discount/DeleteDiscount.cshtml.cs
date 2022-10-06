using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Pages.Admin.Discount
{
    public class DeleteDiscountModel : PageModel
    {
        private IOrderService _orderService;
        public DeleteDiscountModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty]
        public TopCourse.DataLayer.Entities.Order.Discount Discount { get; set; }
        public void OnGet(int id)
        {
            Discount = _orderService.GetDiscountById(id);
        }

        public IActionResult OnPost(int discountId)
        {
            _orderService.DeleteDiscount(discountId);
            return RedirectToPage("IndexCourse");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Pages.Admin.Discount
{
    public class IndexModel : PageModel
    {
        private IOrderService _orderService;
        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty]
        public List<TopCourse.DataLayer.Entities.Order.Discount> Discount { get; set; }
        public void OnGet()
        {
            Discount = _orderService.GetAllDiscount();
        }
    }
}
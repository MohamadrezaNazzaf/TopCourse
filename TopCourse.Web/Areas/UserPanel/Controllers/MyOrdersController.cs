using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopCourse.Core.DTOs.Order;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class MyOrdersController : Controller
    {
        private IOrderService _orderService;
        public MyOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View(_orderService.GetUserOrders(User.Identity.Name));
        }

        public IActionResult ShowOrder(int id, bool finaly=false, string type="")
        {
            var order = _orderService.GetOrderForUserPanel(User.Identity.Name, id);
            if(order == null)
            {
                return NotFound();
            }
            ViewBag.typeDiscount = type;
            ViewBag.finaly = finaly;
            return View(order);
        }

        public IActionResult FinalyOrder(int id)
        {
            if(_orderService.FinalyOrder(User.Identity.Name, id))
            {
                return Redirect("/UserPanel/MyOrders/ShowOrder/"+id+"?finaly=true");
            }
            return BadRequest();
        }

        public IActionResult UseDiscount(int orderId, string code, List<int> courseId)
        {
            DiscountUseType type = _orderService.UseDiscount(orderId, code, courseId);
            return Redirect("/UserPanel/MyOrders/ShowOrder/"+orderId+"?type="+type.ToString());
        }

        public IActionResult ReduceCount(int id)
        {
            var orderId = _orderService.GetOrderIdByDetailId(id);
            _orderService.ReduceCountOrderDetial(id);
            return Redirect("/UserPanel/MyOrders/ShowOrder/" + orderId);
        }
    }
}
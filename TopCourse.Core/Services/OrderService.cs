using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCourse.Core.DTOs.Order;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Context;
using TopCourse.DataLayer.Entities.Course;
using TopCourse.DataLayer.Entities.Order;
using TopCourse.DataLayer.Entities.User;
using TopCourse.DataLayer.Entities.Wallet;

namespace TopCourse.Core.Services
{
    public class OrderService : IOrderService
    {
        private TopCourseContext _context;
        private IUserService _userService;
        public OrderService(TopCourseContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public int AddOrder(string userName, int courseId)
        {
            int userId = _userService.GetUserIdByUserName(userName);

            Order order = _context.Orders.FirstOrDefault(o => o.UserId == userId && !o.IsFinaly); // o.IsFinaly false bashad - factor baste nabashe

            var course = _context.Courses.Find(courseId);

            if (order == null)
            {
                order = new Order()
                {
                    UserId = userId,
                    IsFinaly = false,
                    CreateDate = DateTime.Now,
                    OrderSum = course.CoursePrice,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId = courseId,
                            Count = 1,
                            Price = course.CoursePrice
                        }
                    }
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            else
            {
                OrderDetail detail = _context.OrderDetails.FirstOrDefault(d => d.OrderId == order.OrderId && d.CourseId == courseId);
                if (detail != null)
                {
                    detail.Count += 1;
                    _context.OrderDetails.Update(detail);
                }
                else
                {
                    detail = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        Count = 1,
                        CourseId = courseId,
                        Price = course.CoursePrice
                    };
                    _context.OrderDetails.Add(detail);
                }
                _context.SaveChanges();
                UpdatePriceOrder(order.OrderId);
            }



            return order.OrderId;
        }

        public void UpdatePriceOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            order.OrderSum = _context.OrderDetails.Where(d => d.OrderId == orderId).Sum(d => d.Price * d.Count);
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public Order GetOrderForUserPanel(string userName, int orderId)
        {
            int userId = _userService.GetUserIdByUserName(userName);

            return _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);
        }

        public bool FinalyOrder(string userName, int orderId)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            var order = _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Course).FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);

            if (order == null || order.IsFinaly)
            {
                return false;
            }

            if (_userService.BalanceUserWallet(userName) >= order.OrderDetails.Sum(od => od.Price * od.Count))
            {
                order.IsFinaly = true;
                _userService.AddWallet(new Wallet()
                {
                    Amount = order.OrderSum,
                    CreateDate = DateTime.Now,
                    IsPay = true,
                    Description = "فاکتور شماره #" + order.OrderId,
                    UserId = userId,
                    TypeId = 2
                });
                _context.Orders.Update(order);

                foreach (var detail in order.OrderDetails) // Course haei ke dar factor hast ro be UserCourse ezafe mikone.
                {
                    if(detail.Count >= 1)
                    _context.UserCourses.Add(new UserCourse()
                    {
                        CourseId = detail.CourseId,
                        UserId = userId
                    });
                }

                _context.SaveChanges();
                return true;
            }
            return false;

        }

        public List<Order> GetUserOrders(string userName)
        {
            var userId = _userService.GetUserIdByUserName(userName);
            return _context.Orders.Where(o => o.UserId == userId).ToList();
        }

        public DiscountUseType UseDiscount(int orderId, string code, List<int> courseId)
        {
            var discount = _context.Discounts.SingleOrDefault(d => d.DiscountCode == code);

            string[] timeNow = DateTime.Now.ToString().Split(' ');
            string[] startDate = discount.StartDate.ToString().Split(' ');
            string[] endDate = discount.EndDate.ToString().Split(' ');

            foreach(var item in courseId)
            {
                if(_context.CourseDiscounts.Any(c=>c.CourseId != item))
                {
                    return DiscountUseType.NotHave;
                }
            }

            if (discount == null)
                return DiscountUseType.NotFound;

            if (discount.StartDate != null && DateTime.Parse(startDate[0]) > DateTime.Parse(timeNow[0]))
                return DiscountUseType.NotStart;

            if (discount.EndDate != null && DateTime.Parse(endDate[0]) < DateTime.Parse(timeNow[0]))
                return DiscountUseType.ExpireDate;

            if (discount.UsableCount != null && discount.UsableCount < 1)
                return DiscountUseType.Finished;


            var order = GetOrderById(orderId);

            if (_context.UserDiscountCodes.Any(d => d.UserId == order.UserId && d.DiscountId == discount.DiscountId))
                return DiscountUseType.UserUsed;

            int percent = (order.OrderSum * discount.DiscountPercent) / 100;
            order.OrderSum = order.OrderSum - percent;

            UpdateOrder(order);

            if (discount.UsableCount != null)
            {
                discount.UsableCount -= 1;
            }
            _context.Discounts.Update(discount);

            _context.UserDiscountCodes.Add(new UserDiscountCode()
            {
                UserId = order.UserId,
                DiscountId = discount.DiscountId
            });

            return DiscountUseType.Success;
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Find(orderId);
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void AddDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            _context.SaveChanges();
        }

        public List<Discount> GetAllDiscount()
        {
            return _context.Discounts.ToList();
        }

        public Discount GetDiscountById(int discountId)
        {
            return _context.Discounts.Find(discountId);
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public bool IsExistCode(string code)
        {
            return _context.Discounts.Any(d => d.DiscountCode == code);
        }

        public bool IsUserInCourse(string userName, int courseId)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            return _context.UserCourses.Any(u => u.UserId == userId && u.CourseId == courseId);
        }

        public int GetOrderIdByDetailId(int orderDetailId)
        {
            return _context.OrderDetails.FirstOrDefault(od => od.DetailId == orderDetailId).OrderId;
        }

        public void ReduceCountOrderDetial(int orderDetailId)
        {
            OrderDetail detail = _context.OrderDetails.FirstOrDefault(d => d.DetailId == orderDetailId);
            var order = GetOrderIdByDetailId(orderDetailId);
            detail.Count -= 1;
            _context.OrderDetails.Update(detail);
            _context.SaveChanges();

            if (detail.Count == 0)
            {
                DeleteOrderDetail(orderDetailId);
            }

            UpdatePriceOrder(order);
        }

        public void DeleteOrderDetail(int orderDetailId)
        {
            var detail = _context.OrderDetails.Find(orderDetailId);
            _context.Entry(detail).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public bool IsExistOrderForUser(string userName)
        {
            var userId = _userService.GetUserIdByUserName(userName);
            var order = _context.Orders.SingleOrDefault(o => o.UserId == userId && o.IsFinaly == false);
            if (order != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetOrderIdOpenByUserName(string userName)
        {
            var userId = _userService.GetUserIdByUserName(userName);
            return _context.Orders.FirstOrDefault(o=>o.UserId == userId&& !o.IsFinaly).OrderId;
        }

        public void DeleteDiscount(int discountId)
        {
            var discount = GetDiscountById(discountId);
            var userDiscountCode = _context.UserDiscountCodes.Where(u => u.DiscountId == discountId).ToList();
            foreach(var item in userDiscountCode)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }
            _context.Entry(discount).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public string GetDiscountCodeById(int id)
        {
            return _context.Discounts.FirstOrDefault(d=>d.DiscountId == id).DiscountCode;
        }

        public void AddCourseDiscount(List<int> courseId, int discountId)
        {
            foreach(var item in courseId)
            {
                _context.CourseDiscounts.Add(new CourseDiscount
                {
                    CourseId = item,
                    DiscountId = discountId
                });
            }
            _context.SaveChanges();
        }
    }
}

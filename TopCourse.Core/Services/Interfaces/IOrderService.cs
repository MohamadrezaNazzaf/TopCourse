using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.Core.DTOs.Order;
using TopCourse.DataLayer.Entities.Order;

namespace TopCourse.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int AddOrder(string userName, int courseId);
        void UpdatePriceOrder(int orderId);
        Order GetOrderForUserPanel(string userName, int orderId);
        Order GetOrderById(int orderId);
        bool FinalyOrder(string userName, int orderId);
        List<Order> GetUserOrders(string userName);
        void UpdateOrder(Order order);
        bool IsUserInCourse(string userName, int courseId);
        int GetOrderIdByDetailId(int orderDetailId);
        void ReduceCountOrderDetial(int orderDetailId);
        void DeleteOrderDetail(int orderDetailId);
        bool IsExistOrderForUser(string userName);
        int GetOrderIdOpenByUserName(string userName);

        #region DisCount
        DiscountUseType UseDiscount(int orderId, string code, List<int> courseId);
        void AddDiscount(Discount discount);
        List<Discount> GetAllDiscount();
        Discount GetDiscountById(int discountId);
        void UpdateDiscount(Discount discount);
        void DeleteDiscount(int discountId);
        bool IsExistCode(string code);
        string GetDiscountCodeById(int id);
        void AddCourseDiscount(List<int> courseId, int discountId);
        #endregion
    }
}

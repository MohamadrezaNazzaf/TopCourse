using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCourse.Core.Services.Interfaces;
using TopCourse.DataLayer.Context;
using TopCourse.DataLayer.Entities.Permissions;
using TopCourse.DataLayer.Entities.User;

namespace TopCourse.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private TopCourseContext _context;
        private IUserService _userService;
        public PermissionService(TopCourseContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public List<Role> GetRole()
        {
            return _context.Roles.ToList();
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach(int roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
            _context.SaveChanges();
        }

        public void EditRolesToUser(List<int> roleIds, int userId)
        {
            // Delete All Role User
            _context.UserRoles.Where(r => r.UserId == userId).ToList().ForEach(r=> _context.UserRoles.Remove(r));

            // Add New Role
            AddRolesToUser(roleIds,userId);
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public Role GetRoleById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void DeleteRole(Role role)
        {
            role.IsDelete = true;
            UpdateRole(role);
        }

        public List<Permission> GetAllPermision()
        {
            return _context.Permission.ToList();
        }

        public void AddPermissionToRole(int roleId, List<int> permission)
        {
           foreach(var p in permission)
            {
                _context.RolePermission.Add(new RolePermission
                {
                    PermissionId = p,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public List<int> PermissionsRole(int roleId)
        {
            return _context.RolePermission.Where(r => r.RoleId == roleId).Select(r => r.PermissionId).ToList();
        }

        public void UpdatePermissionsRole(int roleId, List<int> permissions)
        {
            _context.RolePermission.Where(p => p.RoleId == roleId)
                .ToList().ForEach(p=> _context.RolePermission.Remove(p));

            AddPermissionToRole(roleId, permissions);
        }

        public bool CheckPermission(int permissionId, string username) // ببین آیا کاربر نقشی داره؟ بعد اون نقش این پرمیشن رو داره؟
        {
            int userId = _context.Users.Single(u=>u.UserName == username).UserId; //آیدی کاربر رو بده


            List<int> UserRoles = _context.UserRoles
                .Where(r => r.UserId == userId).Select(r=>r.RoleId).ToList();//لیست نقش های این کاربر رو بده


            if (!UserRoles.Any())
                return false; // اگر نقشی نداشت کاربر، فالس برگردون

            List<int> RolesPermission = _context.RolePermission // لیست نقشهایی که این پرمیشن رو دارن بدست بیار
                .Where(p => p.PermissionId == permissionId)
                .Select(p => p.RoleId).ToList(); // ورودی یه کد دسترسی، مثلا مدیریت نقش ها، داریم برحسب اون میره نقش هایی که این دسترسی رو دارن میاره

            return RolesPermission.Any(p => UserRoles.Contains(p)); // حالا از توی نقشایی که با این کد دسترسی فیلتر شدن ببین کاربر همچین نقشایی داره یا نه
             
        }

        public List<int> GetUserRoleById(string userName)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            return _context.UserRoles.Where(r => r.UserId == userId).Select(r=>r.RoleId).ToList(); // نقش های کاربر
        }
    }
}

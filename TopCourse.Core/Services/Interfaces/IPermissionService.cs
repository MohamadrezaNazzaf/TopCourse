using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.DataLayer.Entities.Permissions;
using TopCourse.DataLayer.Entities.User;

namespace TopCourse.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles
        List<Role> GetRole();
        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditRolesToUser(List<int> roleIds, int userId);
        List<int> GetUserRoleById(string userName);
        #endregion

        #region Permission
        List<Permission> GetAllPermision();
        void AddPermissionToRole(int roleId, List<int> permission);
        List<int> PermissionsRole(int roleId);
        void UpdatePermissionsRole(int roleId, List<int> permissions);
        bool CheckPermission(int permissionId, string username);
        #endregion
    }
}

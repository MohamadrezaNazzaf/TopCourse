using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.Core.DTOs;
using TopCourse.DataLayer.Entities.User;
using TopCourse.DataLayer.Entities.Wallet;

namespace TopCourse.Core.Services.Interfaces
{
    public interface IUserService
    {
        bool IsExitUserName(string userName);
        bool IsExitEmail(string email);
        int AddUser(User user);
        User LoginUser(LoginViewModel login);
        User GetUserByEmail(string email);
        User GetUserByUserName(string userName);
        User GetUserById(int userId);
        User GetUserByActiveCode(string activeCode);
        int GetUserIdByUserName(string username);
        int GetUserRole(int userId);
        void UpdateUser(User user);
        bool ActiveAccount(string acticeCode);
        void DeleteUser(int userId);
        void RecoveryUser(int userId);

        #region UserPanel
        InformationUserViewModel GetUserInformation(string username);
        InformationUserViewModel GetUserInformation(int userId);
        InformationUserViewModel GetUserInformationForRecovery(int userId);
        SidebarUserPanelViewModel GetSidebarUserPanelData(string username);
        EditProfileViewModel GetDataForEditProfileUser(string username);
        bool EditProfile(string userName,EditProfileViewModel profile);
        bool CompareOldPassword(string oldPassword,string username);
        void ChangeUserPassword(string userName,string newPassword);
        #endregion

        #region Wallet
        int BalanceUserWallet(string username);
        List<WalletViewModel> GetWalletUser(string username);
        int ChargeWallet(string username, int amount, string description,bool isPay=false);
        int AddWallet(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);
        void UpdateWallet(Wallet wallet);
        #endregion

        #region Admin Panel
        UserForAdminViewModel GetUser(int pageId=1, string filterEmail = "", string filterUserName="");
        UserForAdminViewModel GetDeleteUser(int pageId=1, string filterEmail = "", string filterUserName="");
        int AddUserForAdmin(CreateUserViewModel user);

        EditUserViewModel GetUserForShowInEditMode(int userId);
        void EditUserFromAdmin(EditUserViewModel editUser, bool isActive);
        int UserCount ();
        void SaveImage(User user, IFormFile newAvatar);
        #endregion
    }
}

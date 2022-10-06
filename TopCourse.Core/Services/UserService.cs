
using System;
using System.Collections.Generic;
using System.Text;
using TopCourse.DataLayer.Context;
using TopCourse.Core.Services.Interfaces;
using System.Linq;
using TopCourse.DataLayer.Entities.User;
using TopCourse.Core.DTOs;
using TopCourse.Core.Security;
using TopCourse.Core.Convertors;
using TopCourse.Core.Generator;
using System.IO;
using TopCourse.DataLayer.Entities.Wallet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TopCourse.Core.Utilities.Paging;

namespace TopCourse.Core.Services
{
    public class UserService : IUserService
    {
        private TopCourseContext _context;
        public UserService(TopCourseContext context)
        {
            _context = context;
        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public bool IsExitEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool IsExitUserName(string username)
        {
            return _context.Users.Any(u => u.UserName == username);
        }

        public User LoginUser(LoginViewModel login)
        {
            string hashPassword = PasswordHelper.EncodePasswordMd5(login.Password);
            string email = FixedText.FixEmail(login.Email);
            return _context.Users.SingleOrDefault(u => u.Email == email && u.Password == hashPassword);
        }
        public bool ActiveAccount(string acticeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == acticeCode);
            if (user == null || user.IsActive)
            {
                return false;
            }
            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            _context.SaveChanges();
            return true;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public InformationUserViewModel GetUserInformation(string username)
        {
            var user = GetUserByUserName(username);
            InformationUserViewModel information = new InformationUserViewModel();

            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(username);

            return information;
        }

        public User GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public SidebarUserPanelViewModel GetSidebarUserPanelData(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new SidebarUserPanelViewModel
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDate = u.RegisterDate
            }).Single();
        }

        public EditProfileViewModel GetDataForEditProfileUser(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new EditProfileViewModel
            {
                UserName = u.UserName,
                Email = u.Email,
                AvatarName = u.UserAvatar
            }).Single();
        }

        public bool EditProfile(string userName, EditProfileViewModel profile)
        {
            if (profile.UserAvatar != null)
            {
                string imagePath = "";
                if (profile.AvatarName != "Defult.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                profile.AvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }


            }
            var user = GetUserByUserName(userName);
            user.UserName = profile.UserName;
            user.Email = profile.Email;
            user.UserAvatar = profile.AvatarName;

            UpdateUser(user);
            return true;
        }

        public bool CompareOldPassword(string oldPassword, string username)
        {
            string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return _context.Users.Any(u => u.Password == hashOldPassword && u.UserName == username);
        }

        public void ChangeUserPassword(string userName, string newPassword)
        {
            var user = GetUserByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            UpdateUser(user);
        }

        public int GetUserIdByUserName(string username)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == username).UserId;
        }

        public int BalanceUserWallet(string username)
        {
            var userId = GetUserIdByUserName(username);

            var enter = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay)
                .Select(w => w.Amount).ToList();

            var exit = _context.Wallets
               .Where(w => w.UserId == userId && w.TypeId == 2)
               .Select(w => w.Amount).ToList();

            return (enter.Sum() - exit.Sum());
        }

        public List<WalletViewModel> GetWalletUser(string username)
        {
            int userId = GetUserIdByUserName(username);
            return _context.Wallets
                .Where(w => w.UserId == userId && w.IsPay == true)
                .Select(w => new WalletViewModel
                {
                    Amount = w.Amount,
                    Type = w.TypeId,
                    Description = w.Description,
                    DateTime = w.CreateDate
                }).ToList();
        }

        public int ChargeWallet(string username, int amount, string description, bool isPay)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByUserName(username)
            };
            return AddWallet(wallet);
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public UserForAdminViewModel GetUser(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = _context.Users;

            if (!string.IsNullOrEmpty(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }
            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            //show item in page
            int take = CountShowItemsInPages.Count;
            int skip = (pageId - 1) * take;
            
            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = 0; // تعداد صفحات
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();
            for (int i = 1; i <= _context.Users.Count(); i += take)
            {
                list.PageCount += 1;
            }
            return list;
        }

        public int AddUserForAdmin(CreateUserViewModel user)
        {
            User adduser = new User();

            adduser.UserName = user.UserName;
            adduser.Password = PasswordHelper.EncodePasswordMd5(user.Password);
            adduser.ActiveCode = NameGenerator.GenerateUniqCode();
            adduser.Email = user.Email;
            adduser.IsActive = true;
            adduser.RegisterDate = DateTime.Now;

            #region Save Avatar
            if (user.UserAvatar != null)
            {
                string imagePath = "";

                adduser.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(user.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", adduser.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }

            }
            return AddUser(adduser);
            #endregion

        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new EditUserViewModel
            {
                userId = u.UserId,
                AvatarName = u.UserAvatar,
                Email = u.Email,
                UserName = u.UserName,
                UserRoles = u.UserRoles.Select(r => r.RoleId).ToList(),
                IsActive = u.IsActive
            }).Single();
        }
        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public void SaveImage(User user, IFormFile newAvatar)
        {
            user.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(newAvatar.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", user.UserAvatar);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                newAvatar.CopyTo(stream);
            }
        }
        public void EditUserFromAdmin(EditUserViewModel editUser, bool isActive)
        {
            User user = GetUserById(editUser.userId);
            user.Email = editUser.Email;
            user.IsActive = isActive;
            if (!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(editUser.Password);
            }

            if (editUser.UserAvatar != null)
            {
                //Delete old Image
                if (editUser.AvatarName != "Defult.jpg" && editUser.AvatarName != null)
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUser.AvatarName);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                //Save New Image

                SaveImage(user, editUser.UserAvatar);
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public UserForAdminViewModel GetDeleteUser(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u=>u.IsDelete);

            if (!string.IsNullOrEmpty(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }
            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            //show item in page
            int take = 20;
            int skip = (pageId - 1) * take;

            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take; // تعداد صفحات
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);
            user.IsDelete = true;
            UpdateUser(user);
        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserById(userId);
            InformationUserViewModel information = new InformationUserViewModel();

            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(user.UserName);

            return information;
        }

        public InformationUserViewModel GetUserInformationForRecovery(int userId)
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            InformationUserViewModel information = new InformationUserViewModel();
            List<User> user = result.ToList();
            var users = user.Where(u => u.UserId == userId).Single();
            information.UserName = users.UserName;
            information.Email = users.Email;
           
            return information;
        }

        public void RecoveryUser(int userId)
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            
            List<User> user = result.ToList();
            var users = user.Where(u => u.UserId == userId).Single();
            users.IsDelete = false;
            UpdateUser(users);
        }

        public int GetUserRole(int userId)
        {
            return _context.UserRoles.First(u=>u.UserId == userId).RoleId;
        }

        public int UserCount()
        {
            return _context.Users.Count();
        }
    }
}
using Collectors.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Collectors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collectors.Roles;

namespace Collectors.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private UserManager<IdentityUser> UserManager;
        public AdminController(UserManager<IdentityUser> UserManager)
        {
            this.UserManager = UserManager;
        }
        public async Task<IActionResult> IndexAsync()
        {

            var users = UserManager.Users;
            List<UserModel> userInfo = new List<UserModel>();
            foreach (var user in users)
            {
                string str = "";
                foreach (var role in await UserManager.GetRolesAsync(user))
                    str = (str == "") ? role.ToString() : str + " - " + role.ToString();
                userInfo.Add(new UserModel { User = user, IsBlocked = await UserManager.IsLockedOutAsync(user), Role = str });
            }
            ViewBag.UsersInfo = userInfo;
            var model = new CheckboxListModel { Selected = new List<bool>() };
            for (int i = 0; i < users.Count(); i++)
                model.Selected.Add(false);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
                await UserManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Lock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await UserManager.SetLockoutEnabledAsync(user, true);
                await UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Unlock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await UserManager.SetLockoutEnabledAsync(user, false);
                await UserManager.ResetAccessFailedCountAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MakeAdmin(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await DeleteAllRolesAsync(user);
                await UserManager.AddToRoleAsync(user, UserRoles.admin.ToString());
            }
            return RedirectToAction("Index");
        }

        public async Task DeleteAllRolesAsync(IdentityUser user)
        {
            foreach (var role in await UserManager.GetRolesAsync(user))
            {
                await UserManager.RemoveFromRoleAsync(user, role);
            }
        }

        public async Task<IList<IdentityUser>> GetSelectedUsers(CheckboxListModel model)
        {
            var result = new List<IdentityUser>();
            IdentityUser currentUser = await UserManager.GetUserAsync(HttpContext.User);
            var users = UserManager.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (model.Selected[i] && users[i].Id != currentUser.Id)
                {
                    result.Add(users[i]);
                }
            }
            return result;
        }


    }
}

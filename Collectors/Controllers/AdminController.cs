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
            List<UserModel> usersList = new List<UserModel>();
            await AddUsersAsync(UserManager.Users, usersList);
            ViewBag.Users = usersList;
            var model = new CheckboxListModel { Selected = new List<bool>() };
            foreach(var e in usersList)
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

        private async Task AddUsersAsync(IQueryable<IdentityUser> users, List<UserModel> usersList)
        {
            foreach (var u in users)
            {
                string s = "";
                foreach (var role in await UserManager.GetRolesAsync(u))
                    s = (s == "") ? role.ToString() : s + " - " + role.ToString();
                usersList.Add(new UserModel { User = u, IsBlocked = await UserManager.IsLockedOutAsync(u), Role = s });
            }
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
                    result.Add(users[i]);
            }
            return result;
        }

    }
}

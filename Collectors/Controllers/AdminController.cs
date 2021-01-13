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
        private readonly UserManager<IdentityUser> userManager;
        public AdminController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            List<UserModel> usersList = new List<UserModel>();
            await AddUsersAsync(userManager.Users, usersList);
            ViewBag.Users = usersList;
            var model = new CheckboxListModel { Selected = new List<bool>(new bool[usersList.Count]) };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
                await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Lock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await userManager.SetLockoutEnabledAsync(user, true);
                await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Unlock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await userManager.SetLockoutEnabledAsync(user, false);
                await userManager.ResetAccessFailedCountAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MakeAdmin(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await DeleteAllRolesAsync(user);
                await userManager.AddToRoleAsync(user, UserRoles.admin.ToString());
            }
            return RedirectToAction("Index");
        }

        private async Task AddUsersAsync(IQueryable<IdentityUser> users, List<UserModel> usersList)
        {
            foreach (var u in users)
            {
                string s = "";
                foreach (var role in await userManager.GetRolesAsync(u))
                    s = (s == "") ? role.ToString() : s + " - " + role.ToString();
                usersList.Add(new UserModel { User = u, IsBlocked = await userManager.IsLockedOutAsync(u), Role = s });
            }
        }

        public async Task DeleteAllRolesAsync(IdentityUser user)
        {
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }

        public async Task<IList<IdentityUser>> GetSelectedUsers(CheckboxListModel model)
        {
            var result = new List<IdentityUser>();
            IdentityUser currentUser = await userManager.GetUserAsync(HttpContext.User);
            var users = userManager.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (model.Selected[i] && users[i].Id != currentUser.Id)
                    result.Add(users[i]);
            }
            return result;
        }

    }
}

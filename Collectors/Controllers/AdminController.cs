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
using Collectors.Classes;

namespace Collectors.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ModelsBuilder _modelBuilder = new ModelsBuilder();
        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            List<UserModel> usersList = new List<UserModel>();
            await AddUsersAsync(_userManager.Users, usersList);
            ViewBag.Users = usersList;
            var model = new CheckboxListModel { Selected = new List<bool>(new bool[usersList.Count]) };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
                await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Lock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Unlock(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await _userManager.SetLockoutEnabledAsync(user, false);
                await _userManager.ResetAccessFailedCountAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MakeAdmin(CheckboxListModel model)
        {
            foreach (var user in await GetSelectedUsers(model))
            {
                await DeleteAllRolesAsync(user);
                await _userManager.AddToRoleAsync(user, UserRoles.admin.ToString());
            }
            return RedirectToAction("Index");
        }

        public async Task DeleteAllRolesAsync(IdentityUser user)
        {
            foreach (var role in await _userManager.GetRolesAsync(user))
                await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IList<IdentityUser>> GetSelectedUsers(CheckboxListModel model)
        {
            var result = new List<IdentityUser>();
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = _userManager.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (model.Selected[i] && users[i].Id != currentUser.Id)
                    result.Add(users[i]);
            }
            return result;
        }

        private async Task AddUsersAsync(IQueryable<IdentityUser> users, List<UserModel> usersList)
        {
            foreach (var u in users)
            {
                string userRole = "";
                foreach (var role in await _userManager.GetRolesAsync(u))
                    userRole = (userRole == "") ? role.ToString() : userRole + " - " + role.ToString();
                usersList.Add(await _modelBuilder.GetUserModel(_userManager, u, userRole));
            }
        }
    }
}

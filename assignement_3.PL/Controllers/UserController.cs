﻿using assignement_3.BLL.Interfaces;
using assignement_3.DAL.Models;
using assignement_3.PL.dto;
using assignement_3.PL.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace assignement_3.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInManager)
        {
           _userManager = userManager;
            this.
                _roleManager = roleManager;
            this._signInManager = signInManager;
        }
     
        [HttpGet]
        public async Task<IActionResult> Index(string? SearchInput)
        {
            IEnumerable<UserToReturnDto> user;

            if (SearchInput == null)
            {
                 user = _userManager.Users.Select(u => new UserToReturnDto()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Id = u.Id,
                    UserName= u.UserName,
                    Roles = _userManager.GetRolesAsync(u).Result,
                });
                    
                    }
            else
            {
                user = _userManager.Users.Select(u => new UserToReturnDto()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = _userManager.GetRolesAsync(u).Result,
                }).Where(u=>u.FirstName.ToLower().Contains(SearchInput.ToLower()));
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? SearchInput)
        {
            IEnumerable<UserToReturnDto> user;

            if (SearchInput == null)
            {
                user = _userManager.Users.Select(u => new UserToReturnDto()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = _userManager.GetRolesAsync(u).Result,
                });

            }
            else
            {
                user = _userManager.Users.Select(u => new UserToReturnDto()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = _userManager.GetRolesAsync(u).Result,
                }).Where(u => u.FirstName.ToLower().Contains(SearchInput.ToLower()));
            }

            return PartialView("UsersPartials/_UserTablePartial", user);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {

            if (id is null) return BadRequest(error: "Invalid Id"); // 400

            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound(new { statusCode = 404, message = $"User With Id :{id} is not found" });
            ViewData["userRole"] = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var dto = new UserToReturnDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result,
            };

            return View(viewName, dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id is null) return BadRequest(error: "Invalid Id"); // 400

            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound(new { statusCode = 404, message = $"User With Id :{id} is not found" });
            ViewData["userRole"] = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return await Details(id, "Edit");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserToReturnDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.Role);
            if (role is null)
                return NotFound();
            if (ModelState.IsValid)
            {

                if (id != model.Id) return BadRequest(error: "Invalid Operations !");

                var user = await _userManager.FindByIdAsync(id);
                if (user is null) return BadRequest(error: "Invalid Operations !");
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRolesAsync(user, _userManager.GetRolesAsync(user).Result);
                    await _userManager.AddToRoleAsync(user, role.Name);
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {

            if (id is null) return BadRequest(error: "Invalid Id"); // 400

            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound(new { statusCode = 404, message = $"User With Id :{id} is not found" });
            ViewData["userRole"] = _userManager.GetRolesAsync(user).Result.FirstOrDefault();


            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string id, UserToReturnDto model)
        {

            if (ModelState.IsValid)
            {

                if (id != model.Id) return BadRequest(error: "Invalid Operations !");

                var user = await _userManager.FindByIdAsync(id);
                if (user is null) return BadRequest(error: "Invalid Operations !");
            

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)

                    return RedirectToAction(nameof(Index));
            }

            return View();
        }


      

    }
}

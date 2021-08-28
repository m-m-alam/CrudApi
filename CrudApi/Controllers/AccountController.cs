using CrudApi.Data;
using CrudApi.Enities;
using CrudApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register(RegisterModel registerModel)
        {
            bool existRole = await _roleManager.RoleExistsAsync("User");
            if (!existRole)
            {
                // first we create Admin rool    
                var role = new ApplicationRole();
                role.Name = "User";
                await _roleManager.CreateAsync(role);
            }
            if (await UserExists(registerModel.UserName)) return BadRequest("Username is taken");

            var user = new ApplicationUser() { 
                FullName=registerModel.FullName,
                Email=registerModel.Email,
                UserName=registerModel.UserName,
                Address=registerModel.Address,
                
            };

            
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserModel
            {
                FullName =user.FullName,
                Email = user.Email,
                UserName = user.UserName,                
                Address = user.Address
            };
        }



        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login(LoginModel loginModel)
        {
            var user = await _userManager.Users                
                .SingleOrDefaultAsync(x => x.UserName == loginModel.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.Address
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}

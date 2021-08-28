using CrudApi.Data;
using CrudApi.Enities;
using CrudApi.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        [HttpPost("addUser")]
        public async Task<ActionResult<UserModel>> AddUser(RegisterModel registerModel)
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

            var user = new ApplicationUser()
            {
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                UserName = registerModel.UserName,
                Address = registerModel.Address,

            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.Address
            };
        }
        [HttpPost("addAdmin")]
        public async Task<ActionResult<UserModel>> AddAdmin(RegisterModel registerModel)
        {
            bool existRole = await _roleManager.RoleExistsAsync("Admin");
            if (!existRole)
            {
                // first we create Admin rool    
                var role = new ApplicationRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);
            }
            if (await UserExists(registerModel.UserName)) return BadRequest("Username is taken");

            var user = new ApplicationUser()
            {
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                UserName = registerModel.UserName,
                Address = registerModel.Address,

            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.Address
            };
        }

        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser(UserModel model)
        {

            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == model.UserName);
            var updateUser = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.UserName,
                Address = model.Address,
            };
            
            var result = _context.Entry(updateUser).State = EntityState.Modified;

            return Ok("User update Succsessfully");
        }
        [HttpDelete("deleteUser")]
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.DeleteAsync(user);
        }

        [HttpGet("getUsers")]
        
        public async Task<ActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}

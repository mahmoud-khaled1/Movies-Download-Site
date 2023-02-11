using Cinema_Download_Site.Models;
using Cinema_Download_Site.ModelsViews;
using Cinema_Download_Site.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Authentication.Abstractions;
namespace Cinema_Download_Site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        private ApplicationDb _db;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<ApplicationRole> _roleManager;

        public AccountController(ApplicationDb db, UserManager<ApplicationUser> userManager
            ,SignInManager<ApplicationUser> signInManager,RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager=signInManager;
            _roleManager=roleManager;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (EmailExistes(model.Email))
                {
                    return BadRequest("Email is Exist in DB , or not avaliable");
                }
                if (!isEmailValid(model.Email))
                {
                    return BadRequest("Email is not valid ");
                }
                if (UserNameExistes(model.UserName))
                {
                    return BadRequest("UserName is Exist in DB , or not avaliable");

                }
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    

                    // Confirm Email 
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmLink = Url.Action("RegistrationConfirm", "Account", new
                    { ID = user.Id, Token = HttpUtility.UrlEncode(token) }, Request.Scheme);

                    var txt = "please confirm your registration at our website";
                    var link = "<a href=\"" + confirmLink + "\">Confirm Registration </a>";
                    var title = "Registration Confirm";
                    //We use SendGrid API to confirm registration
                    //and send to specific Email the confirmation 
                    if (await SendGridApi.SendEmail(user.Email, user.UserName, txt, link,title))
                    {
                        return StatusCode(StatusCodes.Status200OK);
                    }
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
        [HttpGet]
        [Route("RegistrationConfirm")]
        public async Task<IActionResult> RegistrationConfirm(string ID, string Token)
        {
            if (string.IsNullOrEmpty(ID)|| string.IsNullOrEmpty(Token))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(ID);
            if (user == null)
                return NotFound();
            var result=await _userManager.ConfirmEmailAsync(user,HttpUtility.UrlDecode(Token));
            if (result.Succeeded)
                return Ok("regstration success");
            else
                return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult>Login(LoginModel model)
        {
            await createRoles();
            await CreateAdminUser();
            if (model is null)
                return NotFound();
            var user= await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
                return NotFound();
            //if(!user.EmailConfirmed)
            //{
            //    return Unauthorized("Email is not confirmed yet !");
            //}
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // Add User To specific Role
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    if(!await _userManager.IsInRoleAsync(user, "User"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                }
                var roleName = await getRoleNameByUserId(user.Id);
                if(roleName!=null)
                {
                    AddCookies(user.UserName, await getRoleNameByUserId(user.Id), user.Id, model.RememberMe);
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            else if(result.IsLockedOut)
               return Unauthorized("User Account is locked for 5 mins");
            else
                return BadRequest(result.IsNotAllowed);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await this._signInManager.SignOutAsync();
         
            return Ok("Logout success !");

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            return await _db.Users.ToListAsync();
        }

        [HttpGet]
        [Route("GetRoleName/{email}")]
        public async Task<string> GetRoleName(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var userRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userRole != null)
                {
                    return await _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                }

            }
            return null;

        }





        // Business Methods

        // Cookies to save user data for long time like login as so on ...
        private async void AddCookies(string username, string roleName, string userId, bool remember)
        {

            var claim = new List<Claim> {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier,userId),
            new Claim(ClaimTypes.Role, roleName),
            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                    , new ClaimsPrincipal(claimIdentity), authProperties);

            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                    , new ClaimsPrincipal(claimIdentity), authProperties);
            }

        }
        private bool EmailExistes(string email)
        {
            return _db.Users.Any(u => u.Email == email);
        }
        private bool UserNameExistes(string userName)
        {
            return _db.Users.Any(u => u.UserName == userName);
        }
        private bool isEmailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch(FormatException)
            {
                return false;
            }
        }
        private async Task createRoles()
        {

            if (_roleManager.Roles.Count() < 1)
            {
                // Admin Role
                var adminRole = new ApplicationRole
                {
                    Name = "Admin"
                };
                await _roleManager.CreateAsync(adminRole);

                //User Role
                var userRole = new ApplicationRole
                {
                    Name = "User"
                };
                await _roleManager.CreateAsync(userRole);
            }

        }
        private async Task CreateAdminUser()
        {
            var admin = await _userManager.FindByNameAsync("Admin");
            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    Email = "admin@gmail.com",
                    UserName = "Admin",
                    EmailConfirmed = true
                };
                var x = await _userManager.CreateAsync(user, "Mahmoud123!@#");

                if (x.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync("Admin"))
                        await _userManager.AddToRoleAsync(user, "Admin");

                }
            }
        }
        private async Task<string> getRoleNameByUserId(string userId)
        {
            var userRole =await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            if(userRole!=null)
            {
                return await _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x=>x.Name).FirstOrDefaultAsync();
            }
            return null;
           
        }

       
    }
}

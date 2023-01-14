using Cinema_Download_Site.Models;
using Cinema_Download_Site.ModelsViews;
using Cinema_Download_Site.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Cinema_Download_Site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ApplicationDb _db;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public AccountController(ApplicationDb db, UserManager<ApplicationUser> userManager
            ,SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager=signInManager;
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
                return Ok("Login Success");
            //else if(result.IsLockedOut)
            //    return Unauthorized("User Account is locked for 5 mins");
            else
                return BadRequest(result.IsNotAllowed);
        }








        // Business Methods
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

    }
}

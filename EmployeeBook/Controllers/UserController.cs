using EmployeeBook.AccountService;
using EmployeeBook.Dto;
using EmployeeBook.ImageService;
using EmployeeBook.Models;
using EmployeeBook.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace EmployeeBook.Controllers
{
    public class UserController : Controller
    {
        private IDbContext _dbContext;
        private IImageService _imageService;
        private IAccountService _accountService;
        public  UserController(IDbContext dbContext, IAccountService accountService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DeleteUser(Guid Id)
        {
            _dbContext.DeleteUser(Id);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new UserDto());
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new UserDto());
        }
        [HttpPost]
        public IActionResult Login(UserDto userDto)
        {
            Response.Headers["Cache-Control"] = "no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (ModelState.IsValid)
            { 
            Guid Data = new Guid();
            var returnedUser = _dbContext.GetUser(userDto);
                if (userDto.UserName == returnedUser.Username)
                {
                    if (_accountService.Logins(userDto.UserName, userDto.Password, returnedUser.Password, returnedUser.PasswordSalt))
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(1),
                            SameSite = SameSiteMode.None,
                            Secure = true,
                        };
                        Response.Cookies.Append("Data", Convert.ToString(returnedUser.Id), cookieOptions);
                        return Authenticate(returnedUser);
                    }
                }
            }
            ViewBag.ErrorMessage = "Invalid username/password or user doesn't exist";
            return View("Login");
        }
        [HttpPost]
        public IActionResult CreateUser(UserDto userDto)
        {
            var user = _accountService.CreateAccount(userDto.UserName, userDto.Password);
            if (!_dbContext.CheckUserExist(userDto))
            {
                _dbContext.CreateUser(user);
                return RedirectToAction("Login");
            }
            ViewBag.ErrorMessage = "User already exists, login or sign up using a different user name";
            return View("CreateUser");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditUser(Guid Id)
        {
            var editedUser = new UserEdit();
            editedUser.Id = Id;
            return View(editedUser);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditUser(UserEdit userDto)
        {
            var user = new User();
            user.Id = userDto.Id;
            user.Username= userDto.User.UserName;
            user.Role = userDto.Role;
            if (userDto.User.Password is not null)
            {
                _accountService.CreatePasswordHash(userDto.User.Password, out byte[] newPassword, out byte[] newPasswordHash);
                user.Password = newPassword;
                user.PasswordSalt = newPasswordHash;       
            }
            _dbContext.EditUser(user);
            return RedirectToAction("ListOfUsers");
        }
        public IActionResult Authenticate(User user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, user.Role)
            };
            var userIdentity = new ClaimsIdentity(userClaims, "user Identity");
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            HttpContext.SignInAsync(userPrincipal);
            return RedirectToAction("ListOfProfiles", "Profile");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Data");
            HttpContext.SignOutAsync();
            Response.Headers["Cache-Control"] = "no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return RedirectToActionPermanent("Login","User",new { version = DateTime.Now.Ticks });
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ListOfUsers()
        {
            return View(_dbContext.ShowUsers());
        }
    }
}

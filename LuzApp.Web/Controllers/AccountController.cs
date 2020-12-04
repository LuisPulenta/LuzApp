using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuzApp.Common.Entities;
using LuzApp.Common.Enums;
using LuzApp.Common.Requests;
using LuzApp.Common.Responses;
using LuzApp.Web.Data;
using LuzApp.Web.Data.Entities;
using LuzApp.Web.Helpers;
using LuzApp.Web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LuzApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Neighborhood)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Departments = _combosHelper.GetComboDepartments(),
                Cities = _combosHelper.GetComboCities(0),
                Neighborhoods = _combosHelper.GetComboNeighborhoods(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = string.Empty;

                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.AddUserAsync(model, imagePath, UserType.Admin);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este mail ya está en uso.");
                    model.Departments = _combosHelper.GetComboDepartments();
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    model.Neighborhoods = _combosHelper.GetComboNeighborhoods(model.CityId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(model.Username, "Confirmación de Email", $"<h1>Confirmación de Email</h1>" +
                    $"Para habilitar el Usuario, " +
                    $"por favor haga click en este link:</br></br><a href = \"{tokenLink}\">Confirmación de Email</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar su usuario han sido enviadas por mail.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            model.Departments = _combosHelper.GetComboDepartments();
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            model.Neighborhoods = _combosHelper.GetComboNeighborhoods(model.CityId);
            return View(model);
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o password incorrecto.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Departments = _combosHelper.GetComboDepartments(),
                Cities = _combosHelper.GetComboCities(0),
                Neighborhoods = _combosHelper.GetComboNeighborhoods(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = string.Empty;
                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }




                User user = await _userHelper.AddUserAsync(model, imagePath, UserType.User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.Departments = _combosHelper.GetComboDepartments();
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    model.Neighborhoods = _combosHelper.GetComboNeighborhoods(model.CityId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmación de Email", $"<h1>Confirmación de Email</h1>" +
                    $"Para habilitar el Usuario, " +
                    $"por favor haga click en este link:</br></br><a href = \"{tokenLink}\">Confirmación de Email</a>");

                ViewBag.Message = "Las instrucciones para habilitar su usuario han sido enviadas por mail.";
                return View(model);

            }

            model.Departments = _combosHelper.GetComboDepartments();
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            model.Neighborhoods = _combosHelper.GetComboNeighborhoods(model.CityId);
            return View(model);
        }


        public JsonResult GetCities(int departmentId)
        {
            Department department = _context.Departments
                .Include(c => c.Cities)
                .FirstOrDefault(c => c.Id == departmentId);
            if (department == null)
            {
                return null;
            }

            return Json(department.Cities.OrderBy(d => d.Name));
        }

        public JsonResult GetNeighborhoods(int cityId)
        {
            City city = _context.Cities
                .Include(d => d.Neighborhoods)
                .FirstOrDefault(d => d.Id == cityId);
            if (city == null)
            {
                return null;
            }

            return Json(city.Neighborhoods.OrderBy(c => c.Name));
        }

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            City city = await _context.Cities.FirstOrDefaultAsync(d => d.Neighborhoods.FirstOrDefault(c => c.Id == user.Neighborhood.Id) != null);
            if (city == null)
            {
                city = await _context.Cities.FirstOrDefaultAsync();
            }

            Department department = await _context.Departments.FirstOrDefaultAsync(c => c.Cities.FirstOrDefault(d => d.Id == city.Id) != null);
            if (department == null)
            {
                department = await _context.Departments.FirstOrDefaultAsync();
            }

            EditUserViewModel model = new EditUserViewModel
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImagePath = user.ImagePath,
                Id = user.Id,
                Document = user.Document,

                Departments = _combosHelper.GetComboDepartments(),
                DepartmentId = department.Id,

                Cities = _combosHelper.GetComboCities(department.Id),
                CityId = city.Id,
                
                Neighborhoods = _combosHelper.GetComboNeighborhoods(city.Id),
                NeighborhoodId = user.Neighborhood.Id,
                
              
                
                

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = string.Empty;
                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImagePath = imagePath;
                user.Neighborhood = await _context.Neighborhoods.FindAsync(model.NeighborhoodId);
                
                user.Document = model.Document;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

           
            model.Departments = _combosHelper.GetComboDepartments();
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            model.Neighborhoods = _combosHelper.GetComboNeighborhoods(model.CityId);

            return View(model);
        }
        public IActionResult ChangePasswordMVC()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordMVC(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }
        public IActionResult RecoverPasswordMVC()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPasswordMVC(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "Password Reset", $"<h1>Password Reset</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");
                ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful.";
                    return View();
                }

                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }



    }
}
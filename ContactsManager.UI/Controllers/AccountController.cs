using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFirstApplication.Controllers;
using System.Text.RegularExpressions;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
       [Authorize("NotAuthorized")]
       
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if(ModelState.IsValid == false) 
            {
                ViewBag.Errors = ModelState.Values.SelectMany
                    (temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(registerDTO);
            }
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email,
                PersonName= registerDTO.PersonName
            };

            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

            if (identityResult.Succeeded) 
            {
                if(registerDTO.UserType == Core.Enums.UserTypeOptions.Admin)
                {
                    if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        { Name = UserTypeOptions.Admin.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.Admin.ToString());
                }
                else
                {

                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        { Name = UserTypeOptions.User.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.User.ToString());
                }
                //Sin in
                await _signInManager.SignInAsync(applicationUser,false);
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
            }
            return View(registerDTO);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? returnUrl)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany
                   (temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync
                (loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginDTO.Email);
                if(applicationUser != null)
                {
                    if(await _userManager.IsInRoleAsync(applicationUser, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new {area = "Admin"});
                    }
                }
                if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
            }
            ModelState.AddModelError("Login", "Invalid email or password");
            return View(loginDTO);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons"); 
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
           ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}

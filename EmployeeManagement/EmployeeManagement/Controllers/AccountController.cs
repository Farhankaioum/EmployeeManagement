﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Net;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                                                                            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
       [HttpGet]
       [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token}, Request.Scheme);
                    logger.LogError(confirmationLink);
                    //check login user/admin create another user but still this id login 
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    ViewBag.ErrorMessage = "Thank you for registration, please confirm your email addredd";
                    return View("Error");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
        //for confirm email address
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user id {userId} is invalid";
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, true);
                if(result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        //for external logins
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl});
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //external login redirects user back to our app code
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }
            
            var info = await signInManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login information.");
                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if (email != null)
            {
                user = await userManager.FindByEmailAsync(email);
                if(user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginViewModel);
                }
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, 
                info.ProviderKey, isPersistent:false, bypassTwoFactor:true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);

                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, token = token }, 
                            Request.Scheme);
                        logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your email, by " +
                            "clicking on the confirmation link we have emailed you";
                        return View("Error");
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent:false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact suppoft on admin@tech.com";

                return View("Error");
                
            }
        }

        //Forget password start: [create button, Forgot password all option and Reset password all options ]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token},
                                                                                                                Request.Scheme);
                    logger.Log(LogLevel.Warning, passwordResetLink);

                    //// for mailing user forgot password link using mailKit nugetPackage
                    //var ToMailAddress = userManager.GetEmailAsync(user);

                    //var message = new MimeMessage();
                    //message.From.Add(new MailboxAddress("Md.Keiuom", "kspworld24@gmail.com"));
                    //message.To.Add(new MailboxAddress("User", ToMailAddress.ToString()));
                    //message.Subject = "Password reset link";
                    //message.Body = new TextPart("plain")
                    //{
                    //    Text = "Click is link for reset your password<br/>" + passwordResetLink
                    //};

                    //using (var client = new SmtpClient())
                    //{
                    //    client.Connect("smtp.gmail.com", 587, false);
                    //    client.Authenticate("kspworld24@gmail.com", "visualstudio");

                    //    client.Send(message);
                    //    client.Disconnect(true);

                    //}

                    //for email system
                    var ToMailAddress = userManager.GetEmailAsync(user);
                    var userName = userManager.GetUserNameAsync(user);
                    var confirmEmailMsg = SendEmail(userName.ToString(), ToMailAddress.ToString(), passwordResetLink);
                    if (confirmEmailMsg == "true")
                    {
                        return View("NotFound");
                    }

                    return View("ForgotPasswordConfirmation");

                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if(await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }
        //Forget password end


        // Current Password change start
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                    if(!result.Succeeded)
                     {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View();
                    }
                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }
            return View(model);
        }
        //Current Password change end


        //Add Password  start
        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }
            return View(model);
        }
        //Add Password  end


        //check email, if this email use in another account than cann't use this email for registration in this system
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }
        // for mailing system
        public string SendEmail(string Name, string Email, string Message)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("kspworld24@gmail.com", "kaioum6446560@@##");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("kspworld24@gmail.com"),
                    Subject = "Password Reset Link",
                    Body = Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                // Smtp client
                var client = new System.Net.Mail.SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
                return "true";
            }
            catch (System.Exception e)
            {
                return e.Message;
            }

        }
    }
   

}

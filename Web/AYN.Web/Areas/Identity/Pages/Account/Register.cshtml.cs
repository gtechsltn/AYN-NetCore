﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using AYN.Data.Models;
using AYN.Data.Models.Enumerations;
using AYN.Services.Data.Interfaces;
using AYN.Services.Messaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using static AYN.Common.AttributeConstraints;

namespace AYN.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<RegisterModel> logger;
    private readonly IEmailSender emailSender;
    private readonly ITownsService townsService;
    private readonly IWebHostEnvironment environment;
    private readonly IUsersService usersService;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        ITownsService townsService,
        IWebHostEnvironment environment,
        IUsersService usersService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
        this.townsService = townsService;
        this.environment = environment;
        this.usersService = usersService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public IEnumerable<KeyValuePair<string, string>> Towns { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Username")]
        [MinLength(ApplicationUserUserNameMinLength)]
        [MaxLength(ApplicationUserUserNameMaxLength)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must be combination of letters and numbers only.")]
        public string UserName { get; set; }

        [Required]
        [MinLength(ApplicationUserAboutMinLength)]
        [MaxLength(ApplicationUserAboutMaxLength)]
        public string About { get; set; }

        [Required]
        [MinLength(ApplicationUserFirstNameMinLength)]
        [MaxLength(ApplicationUserFirstNameMaxLength)]
        public string FirstName { get; set; }

        [MinLength(ApplicationUserMiddleNameMinLength)]
        [MaxLength(ApplicationUserMiddleNameMaxLength)]
        public string MiddleName { get; set; }

        [Required]
        [MinLength(ApplicationUserLastNameMinLength)]
        [MaxLength(ApplicationUserLastNameMaxLength)]
        public string LastName { get; set; }

        [Required]
        public int TownId { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        this.ReturnUrl = returnUrl;
        this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        this.Towns = await this.townsService.GetAllAsKeyValuePairsAsync();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= this.Url.Content("~/");

        this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        this.Towns = await this.townsService.GetAllAsKeyValuePairsAsync();

        if (this.usersService.IsEmailTaken(this.Input.Email))
        {
            this.ModelState.AddModelError(string.Empty, "Email taken.");
        }

        if (this.ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = this.Input.UserName,
                Email = this.Input.Email,
                About = this.Input.About,
                FirstName = this.Input.FirstName,
                LastName = this.Input.LastName,
                TownId = 1,
                Gender = this.Input.Gender,
                ThumbnailImageUrl = await this.usersService.GenerateDefaultThumbnail(this.Input.FirstName, this.Input.LastName),
                AvatarImageUrl = await this.usersService.GenerateDefaultAvatar(this.Input.FirstName, this.Input.LastName),
            };

            var result = await this.userManager.CreateAsync(user, this.Input.Password);

            if (result.Succeeded)
            {
                this.logger.LogInformation("User created a new account with password.");

                var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = this.Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: this.Request.Scheme);

                await this.emailSender.SendEmailAsync(
                    "allyouneedplatform@gmail.com",
                    "AYNPlatform",
                    this.Input.Email,
                    "Confirm your email",
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here</a> to confirm your AYN account.");

                if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return this.RedirectToPage("RegisterConfirmation", new { email = this.Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return this.Page();
    }
}

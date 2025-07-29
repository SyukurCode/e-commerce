// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using E_Commers_Adelia.Service;
using E_Commers_Adelia.Common;

namespace E_Commers_Adelia.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<EUser> _userManager;
        private readonly IEmailService _emailSender;

        public ResendEmailConfirmationModel(UserManager<EUser> userManager, IEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet(string email)
        {
            if (email == null)
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return;
            }
            Input = new InputModel { Email = email };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }
            var randomPassword = RandomPasswordHelper.Generate(); 
            var removePassword = await _userManager.RemovePasswordAsync(user);
            var setNewPassword = await _userManager.AddPasswordAsync(user, randomPassword);
            var userId = await _userManager.GetUserIdAsync(user);
            var userRole = await _userManager.GetRolesAsync(user);
            var whois = userRole[0];
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(Input.Email, "Congratulation!",
                $"<p>This is an auto reply message. Please do not reply to this email.</p>" +
                $"<br/><br/>" +
                $"<p>Dear {whois},</p>" +
                $"<p>Welcome, you has been invaited to join E-commerce Residensi Adelia.</p>" +
                $"<p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>" +
                $"<p>Username: <strong>{Input.Email}</strong></p>" +
                $"<p>Password: <strong>{randomPassword}</strong> (please change your password)</p>" +
                $"<br/><br/>" +
                $"<p>Best Regard,</p>" +
                $"<p><i>System Administrator</i></p>");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}

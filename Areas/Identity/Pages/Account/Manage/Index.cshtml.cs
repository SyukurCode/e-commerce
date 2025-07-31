// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commers_Adelia.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<EUser> _userManager;
        private readonly SignInManager<EUser> _signInManager;

        public IndexModel(
            UserManager<EUser> userManager,
            SignInManager<EUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Display Name")]
            public string DisplayName { get; set; }
            [Display(Name = "Store Name")]
            public string StoreName { get; set; }
            public string Address { get; set; }
        }

        private async Task LoadAsync(EUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var dbUser = await _userManager.GetUserAsync(User);

            var displayName = dbUser.DisplayName; 

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                DisplayName = displayName,
                StoreName = dbUser.StoreName,
                Address = dbUser.Address
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var dbuser = await _userManager.GetUserAsync(User);

            var DisplayName = Input.DisplayName;
            if(DisplayName != dbuser.DisplayName)
            {
                dbuser.DisplayName = DisplayName;
                var setDisplayName = await _userManager.UpdateAsync(dbuser);
                if (!setDisplayName.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set display name.";
                    return RedirectToPage();
                }
            }
            var StoreName = Input.StoreName;
            if (StoreName != dbuser.StoreName)
            {
                dbuser.StoreName = StoreName; 
                var setStoreName = await _userManager.UpdateAsync(dbuser);
                if (!setStoreName.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set store name.";
                    return RedirectToPage();
                }
            }
            var Address = Input.Address;
            if (Address != dbuser.Address)
            {
                dbuser.Address = Address;
                var setAddress = await _userManager.UpdateAsync(dbuser);
                if (!setAddress.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set address.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

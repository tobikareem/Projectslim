// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseImage _imageBase;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IBaseImage baseImage)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imageBase = baseImage;
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
        public AddressModel Input { get; set; }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new AddressModel
            {
                PhoneNumber = phoneNumber
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

            //if (Input.ProfileImage != null)
            //{
            //    try
            //    {
            //        var image = await UploadImageAsync();

            //        if (image == default(Image) || !string.IsNullOrWhiteSpace(StatusMessage))
            //        {
            //            return RedirectToPage();
            //        }

            //        var userCreated = $"profile {User.Identity.Name}";

            //        if (_imageBase.GetAll().FirstOrDefault(x => x.CreatedBy == userCreated && x.IsPrimaryImage) != null)
            //        {
            //            _imageBase.UpdateEntity(image);
            //        }
            //        else
            //        {
            //            _imageBase.AddEntity(image);
            //        }
            //    }
            //    catch (Exception)
            //    {

            //        StatusMessage = "Error uploading profile image";
            //        return RedirectToPage();
            //    }

            //}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        //private async Task<Image> UploadImageAsync()
        //{
        //    var image = new Image();
        //    try
        //    {

        //        await using var ms = new MemoryStream();
        //        await Input.ProfileImage.CopyToAsync(ms);


        //        // upload the file if less than 2 MB
        //        if (ms.Length < 2097152)
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(Input.ProfileImage.ContentDisposition).FileName.Trim();
        //            var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
        //            var fileExtension = fileName.Value[(index + 1)..];

        //            if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg")
        //            {
        //                image.ImageId = Guid.NewGuid();
        //                image.UploadedImage = ms.ToArray();
        //                image.IsPrimaryImage = true;
        //                image.Enabled = true;
        //                image.CreatedDate = DateTime.UtcNow;
        //                image.CreatedBy = $"profile {User.Identity.Name}";
        //            }
        //            else
        //            {
        //                StatusMessage = "Please upload a valid image file (jpg, png, jpeg)";
        //            }

        //        }
        //        else
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(Input.ProfileImage.ContentDisposition).FileName.Trim();
        //            StatusMessage = $"The file {fileName} is too large. Must be less than 2 MB";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        StatusMessage = "Error uploading file for product" + e.Message;
        //        throw;
        //    }

        //    return image;
        //}
    }
}

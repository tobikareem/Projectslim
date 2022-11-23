// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseStore<UserPageImage> _userPageImageRepository;
        private readonly IUserService _userService;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IBaseStore<UserPageImage> userPageImageRepository,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userPageImageRepository = userPageImageRepository;
            _userService = userService;
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
                Email = user.Email,
                PhoneNumber = phoneNumber,
                Address1 = User.FindFirstValue(ClaimTypes.StreetAddress),
                Address2 = User.FindFirstValue(CustomClaims.Address2),
                ZipCode = User.FindFirstValue(CustomClaims.Zipcode),
                ProfileImage = GetProfileImage(),
                BillingAddress1 = User.FindFirstValue(CustomClaims.BillingAddress1),
                BillingAddress2 = User.FindFirstValue(CustomClaims.BillingAddress2),
                BillingZipCode = User.FindFirstValue(CustomClaims.BillingZipCode),
                IsSameAsAddress = Convert.ToBoolean(User.FindFirstValue(CustomClaims.IsSameAsAddress))
            };
        }

        private IFormFile GetProfileImage()
        {
            var userPageImage = _userPageImageRepository.GetAll().FirstOrDefault(x =>
                x.CreatedBy == User.Identity?.Name && x.ImageDescription == "Profile Image");
            if (userPageImage == null)
            {
                return null;
            }

            var file = new FormFile(new MemoryStream(userPageImage.UploadedImage), 0, userPageImage.UploadedImage.Length, userPageImage.ImageId.ToString(), userPageImage.ImageId.ToString())
            {
                Headers = new HeaderDictionary(),
                ContentType = userPageImage.ImageName ?? "jpg"
            };
            return file;
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
            

            if (!string.IsNullOrWhiteSpace(Input.Address1))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, ClaimTypes.StreetAddress, Input.Address1);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Address1.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.Address2))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.Address2, Input.Address2);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Address2.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.ZipCode))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.Zipcode, Input.ZipCode);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Zipcode.";
                    return RedirectToPage();
                }
            }
            var isSame = await _userService.UpsertUserClaim(user, CustomClaims.IsSameAsAddress, Input.IsSameAsAddress.ToString());
            if (!isSame)
            {
                StatusMessage = "Unable to select this option for same address";
                return RedirectToPage();
            }

            if (!string.IsNullOrWhiteSpace(Input.BillingAddress1))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingAddress1, Input.BillingAddress1);
                if (!isSuccess)
                {
                    StatusMessage = "Unable to replace Billing Address1.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.BillingAddress2))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingAddress2, Input.BillingAddress2);
                if (!isSuccess)
                {
                    StatusMessage = "Unable to replace Billing Address2.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.BillingZipCode))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingZipCode, Input.BillingZipCode);
                if (!isSuccess)
                {
                    StatusMessage = "Unable to replace Billing Zip code.";
                    return RedirectToPage();
                }

            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Input.ProfileImage != null)
            {
                try
                {
                    var image = await UploadImageAsync();

                    if (image == default(UserPageImage) || !string.IsNullOrWhiteSpace(StatusMessage))
                    {
                        return RedirectToPage();
                    }


                    if (_userPageImageRepository.GetAll().FirstOrDefault(x => x.CreatedBy == User.Identity?.Name && x.ImageDescription == "Profile Image") != null)
                    {
                        _userPageImageRepository.UpdateEntity(image);
                    }
                    else
                    {
                        _userPageImageRepository.AddEntity(image);
                    }
                }
                catch (Exception)
                {

                    StatusMessage = "Error uploading profile image";
                    return RedirectToPage();
                }

            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task<UserPageImage> UploadImageAsync()
        {
            var image = new UserPageImage();
            try
            {

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Input.ProfileImage == null)
                {
                    return image;
                }

                await using var ms = new MemoryStream();
                await Input.ProfileImage.CopyToAsync(ms);


                // upload the file if less than 2 MB
                if (ms.Length < 2097152)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(Input.ProfileImage.ContentDisposition).FileName.Trim();
                    var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
                    var fileExtension = fileName.Value[(index + 1)..];

                    if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg" or "heic")
                    {
                        image.ImageId = Guid.NewGuid();
                        image.UploadedImage = ms.ToArray();
                        image.ImageDescription = "Profile Image";
                        image.Enabled = true;
                        image.ImageName = fileExtension;
                        image.CreatedDate = DateTime.UtcNow;
                        image.CreatedBy = User.Identity?.Name;
                    }
                    else
                    {
                        StatusMessage = "Please upload a valid image file (jpg, png, jpeg)";
                    }

                }
                else
                {
                    var fileName = ContentDispositionHeaderValue.Parse(Input.ProfileImage.ContentDisposition).FileName.Trim();
                    StatusMessage = $"The file {fileName} is too large. Must be less than 2 MB";
                }
            }
            catch (Exception e)
            {
                StatusMessage = "Error uploading file for product" + e.Message;
                throw;
            }

            return image;
        }
    }
}

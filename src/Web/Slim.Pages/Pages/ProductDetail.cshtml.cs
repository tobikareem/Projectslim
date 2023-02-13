using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Packaging;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages;
public class ProductDetailModel : PageModel
{
    private readonly IBaseStore<Product> _productBaseStore;
    private readonly IBaseStore<Review> _reviewBaseStore;
    private readonly IBaseStore<Comment> _commentBaseStore;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBaseStore<UserPageImage> _userPageImage;
    private readonly IBaseStore<RazorPage> _razorPagesBaseStore;

    private ILogger<ProductDetailModel> _logger;
    private ICacheService _cacheService;
    private readonly ICartService _cartService;
    public string[] BagSizes = SlmConstant.BagSizes;
    public bool IsBagCategory;

    public ProductDetailModel(
        IBaseStore<Product> productBaseStore,
        ILogger<ProductDetailModel> logger,
        ICacheService cacheService, IBaseStore<Review> reviewBaseStore,
        IBaseStore<Comment> commentBaseStore, UserManager<IdentityUser> userManager,
        ICartService cartService,
        IBaseStore<UserPageImage> userPageImage, IBaseStore<RazorPage> razorPagesBaseStore)
    {
        _productBaseStore = productBaseStore;
        _logger = logger;
        _cacheService = cacheService;
        _reviewBaseStore = reviewBaseStore;
        _commentBaseStore = commentBaseStore;
        _userManager = userManager;
        _cartService = cartService;
        _userPageImage = userPageImage;
        _razorPagesBaseStore = razorPagesBaseStore;

        Product = new Product();
    }

    [BindProperty] public Product Product { get; set; }
    [BindProperty] public ReviewModel Review { get; set; } = new();
    [BindProperty] public string UserComment { get; set; } = string.Empty;
    [BindProperty] public string SizeSelectionRadioButton { get; set;} = string.Empty;
    [TempData] public string TempComment { get; set; } = string.Empty;
    [TempData] public string TempReview { get; set; } = string.Empty;
    [TempData] public string StatusMessage { get; set; } = string.Empty;
    public byte[]? ProfileImage { get; set; }


    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product = GetProductAndProfilePicture(id.Value);

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            Review.FullName = $"{User.Claims.Where(x => x.Type == ClaimTypes.Name).Skip(1).FirstOrDefault()?.Value} {User.FindFirstValue(ClaimTypes.Surname)}";
            Review.Email = user.Email;
        }

        var cartItem = _cartService.GetCartItemsForUser(User.Identity?.Name, GetCartUserId()).FirstOrDefault(x => x.ProductId == id);
        

        if (!Product.ProductDetails.Any(x => x.HasMini))
        {
            BagSizes = BagSizes.Where(x => x != "Mini").ToArray();
        }

        if (!Product.ProductDetails.Any(x => x.HasMidi))
        {
            BagSizes = BagSizes.Where(x => x != "Midi").ToArray();
        }

        if (!Product.ProductDetails.Any(x => x.HasMaxi))
        {
            BagSizes = BagSizes.Where(x => x != "Maxi").ToArray();
        }


        if (cartItem != null)
        {
            SizeSelectionRadioButton = BagSizes.FirstOrDefault(x => x == cartItem.BagSize) ?? string.Empty;
        }

        var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);

        IsBagCategory = razorPages.First(x => x.PageName == "Bags").Id == Product.RazorPageId;


        if (TempData["TempReview"] == null && TempData["TempComment"] == null)
        {
            return Page();
        }

        if (TempData["TempComment"] != null)
        {
            _logger.LogInformation("User {user} is redirected from Login Page for their Comment", User.Identity?.Name);
            return await OnPostUserCommentSubmit();
        }

        _logger.LogInformation("User {user} is redirected from Login Page for their Review", User.Identity?.Name);
        return OnPostSubmitUserReview();
    }

    public async Task<IActionResult> OnPostUserCommentSubmit()
    {
        var userComment = new Comment
        {
            CreatedDate = DateTime.UtcNow,
            UserComment = UserComment,
            ProductId = Product.Id
        };

        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            TempComment = UserComment;
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = $"/ProductDetail/{Product.Id}" });
        }

        if (TempData["TempComment"] != null)
        {
            userComment.UserComment = Convert.ToString(TempData["TempComment"]) ?? string.Empty;
        }

        var user = await _userManager.GetUserAsync(User);
        userComment.CreatedBy = User.Identity.Name;
        userComment.FullName = $"{User.Claims.Where(x => x.Type == ClaimTypes.Name).Skip(1).FirstOrDefault()?.Value} {User.FindFirstValue(ClaimTypes.Surname)}";
        userComment.Email = user.Email;

        _commentBaseStore.AddEntity(userComment, CacheKey.GetComments, true);

        Product = GetProductAndProfilePicture(Product.Id);

        if (Product.Comments.Any() && Product.Comments.Any(x => x.Id != userComment.Id))
        {
            Product.Comments.Add(userComment);
        }
        else
        {
            var comments = _cacheService.GetOrCreate(CacheKey.GetComments, () => _commentBaseStore.GetAll()).Where(x => x.ProductId == Product.Id).ToList();
            Product.Comments.AddRange(comments);
        }
        TempData.Clear();
        return RedirectToPage("/ProductDetail", new { id = Product.Id });
    }

    public IActionResult OnPostSubmitUserReview()
    {
        var userReview = new Review
        {
            CreatedDate = DateTime.UtcNow,
            ProductId = Product.Id,
            Rating = Review.Rating,
            UserReview = Review.UserReview,
            Pros = Review.Pros,
            Cons = Review.Cons,
            FullName = Review.FullName,
            Email = Review.Email
        };

        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            TempReview = JsonConvert.SerializeObject(userReview);
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = $"/ProductDetail/{Product.Id}" });
        }

        var isUserRedirectedReview = TempData["TempReview"];

        if (isUserRedirectedReview != null)
        {
            userReview = JsonConvert.DeserializeObject<Review>(Convert.ToString(isUserRedirectedReview) ?? string.Empty);
        }

        if (userReview == null || !ValidateUserReviewModel(userReview))
        {
            Product = GetProductAndProfilePicture(Product.Id);
            return Page();
        }

        userReview.CreatedBy = User.Identity.Name;

        _reviewBaseStore.AddEntity(userReview, CacheKey.GetReviews, true);

        Product = GetProductAndProfilePicture(Product.Id);

        if (Product.Reviews.Any() && Product.Reviews.Any(x => x.Id != userReview.Id))
        {
            Product.Reviews.Add(userReview);
        }
        else
        {
            var reviews = _cacheService.GetOrCreate(CacheKey.GetReviews, () => _reviewBaseStore.GetAll()).Where(x => x.ProductId == Product.Id).ToList();
            Product.Reviews.AddRange(reviews);
        }
            
        TempData.Clear();
        return RedirectToPage("/ProductDetail", new { id = Product.Id });
    }

    private bool ValidateUserReviewModel(Review userReview)
    {
        if (string.IsNullOrWhiteSpace(userReview.FullName))
        {
            StatusMessage = $"Error. {nameof(userReview.FullName)} must have a value";
            return false;
        }

        if (string.IsNullOrWhiteSpace(userReview.Email))
        {
            StatusMessage = $"Error. {nameof(userReview.Email)} must have a value";
            return false;
        }

        if (userReview.Rating == 0)
        {
            StatusMessage = "Error. Please, rate the product";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(userReview.UserReview)) return true;
        StatusMessage = "Error. Please, write a review";
        return false;

    }

    private string GetCartUserId()
    {
        var hasSession = HttpContext.Session.GetString(SlmConstant.SessionKeyName);
        if (string.IsNullOrWhiteSpace(hasSession))
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity?.Name))
            {
                HttpContext.Session.SetString(SlmConstant.SessionKeyName, HttpContext.User.Identity.Name);
            }
            else
            {
                var tempCartId = Guid.NewGuid();
                HttpContext.Session.SetString(SlmConstant.SessionKeyName, tempCartId.ToString());
            }
        }

        var sessionName = HttpContext.Session.GetString(SlmConstant.SessionKeyName);

        return string.IsNullOrEmpty(sessionName) ? string.Empty : sessionName;
    }

    private Product GetProductAndProfilePicture(int id)
    {
        var products = _cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll);
        var product = _cartService.GetProductsWithInCartCheck(products, User.Identity?.Name ?? string.Empty, GetCartUserId()).First(x => x.Id == id);

        if (User.Identity?.Name == null) return product;
        {
            var userImages = _cacheService.GetOrCreate(CacheKey.UserProfileImage, () => _userPageImage.GetAll()).ToList();

            if (userImages.Any())
            {
                ProfileImage = userImages.FirstOrDefault(x => x.CreatedBy == User.Identity?.Name && x.Enabled == true)?.UploadedImage;
            }
        }

        return product;
    }

    public class ReviewModel
    {
        [Required, StringLength(100, MinimumLength = 3), Display(Name = "Full Name"), DataType(DataType.Text), RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 3), Display(Name = "Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 3), Display(Name = "Review"), DataType(DataType.Text)]
        public string UserReview { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Pros { get; set; } = string.Empty;

        public string Cons { get; set; } = string.Empty;
    }
}
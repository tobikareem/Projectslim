using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Packaging;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly IBaseStore<Review> _reviewBaseStore;
        private readonly IBaseStore<Comment> _commentBaseStore;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBaseStore<UserPageImage> _userPageImage;
        private ILogger<ProductDetailModel> _logger;
        private ICacheService _cacheService;
        private readonly ICartService _cartService;

        public ProductDetailModel(
            IBaseStore<Product> productBaseStore,
            ILogger<ProductDetailModel> logger,
            ICacheService cacheService, IBaseStore<Review> reviewBaseStore,
            IBaseStore<Comment> commentBaseStore, UserManager<IdentityUser> userManager,
            ICartService cartService,
            IBaseStore<UserPageImage> userPageImage)
        {
            _productBaseStore = productBaseStore;
            _logger = logger;
            _cacheService = cacheService;
            _reviewBaseStore = reviewBaseStore;
            _commentBaseStore = commentBaseStore;
            _userManager = userManager;
            _cartService = cartService;
            _userPageImage = userPageImage;

            Product = new Product();
        }

        [BindProperty] public Product Product { get; set; }
        [BindProperty] public ReviewModel Review { get; set; } = new();
        [BindProperty] public string UserComment { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
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

            var isUserRedirected = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);
            if (isUserRedirected == null)
            {
                return Page();
            }

            _logger.LogInformation("User {user} is redirected from Login Page", User.Identity?.Name);

            return await OnPostUserCommentSubmit();
            
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

            var isValid = ValidateUserReviewModel(userReview);

            if (!isValid)
            {
                Product = GetProductAndProfilePicture(Product.Id);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Identity?.Name))
            {
                _cacheService.Add(CacheKey.AddThisUsersReview, userReview, 10);
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = $"/ProductDetail/{Product.Id}" });
            }

            var isUserRedirected = _cacheService.GetItem<Review>(CacheKey.AddThisUsersComment);

            if (isUserRedirected != null)
            {
                userReview = _cacheService.GetItem<Review>(CacheKey.AddThisUsersComment);
            }
            
            userReview.CreatedBy = User.Identity.Name;

            _reviewBaseStore.AddEntity(userReview, CacheKey.GetReviews, true);

            if (isUserRedirected != null)
            {
                _cacheService.Remove(CacheKey.AddThisUsersReview);
            }

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
                _cacheService.Add(CacheKey.AddThisUsersComment, userComment, 10);
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = $"/ProductDetail/{Product.Id}" });
            }

            var isUserRedirected = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);

            if (isUserRedirected != null)
            {
                userComment = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);
            }

            var user = await _userManager.GetUserAsync(User);
            userComment.CreatedBy = User.Identity.Name;
            userComment.FullName = $"{User.Claims.Where(x => x.Type == ClaimTypes.Name).Skip(1).FirstOrDefault()?.Value} {User.FindFirstValue(ClaimTypes.Surname)}";
            userComment.Email = user.Email;

            _commentBaseStore.AddEntity(userComment, CacheKey.GetComments, true);

            if (isUserRedirected != null)
            {
                _cacheService.Remove(CacheKey.AddThisUsersComment);
            }

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

            return RedirectToPage("/ProductDetail", new { id = Product.Id });
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
}

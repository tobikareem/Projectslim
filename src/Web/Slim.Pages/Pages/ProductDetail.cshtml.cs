using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        private readonly IBaseCart<ShoppingCart> _baseCart;
        private readonly IBaseStore<UserPageImage> _userPageImage;
        private ILogger<ProductDetailModel> _logger;
        private ICacheService _cacheService;
        private readonly ICartService _cartService;
        public List<ImageData> ImagesToShow { get; set; }
        private string _cartUserId = string.Empty;

        public ProductDetailModel(
            IBaseStore<Product> productBaseStore,
            ILogger<ProductDetailModel> logger,
            IBaseCart<ShoppingCart> baseCart,
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
            _baseCart = baseCart;

            ImagesToShow = new List<ImageData>();
            Product = new Product();
        }

        [BindProperty] public Product Product { get; set; }
        [BindProperty] public ReviewModel Review { get; set; } = new();
        [BindProperty] public UserCommentModel UserComment { get; set; } = new();

        public byte[]? ProfileImage { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =
                _cacheService
                    .GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll)
                    .First(x => x.Id == id.Value);

            _cartUserId = GetCartUserId();

            var cartItem = _baseCart.GetCartUserItem(_cartUserId, id.GetValueOrDefault());
            product.IsProductInCart = false;

            Product = product;

            if (User.Identity?.Name != null)
            {
                var userImages = _cacheService.GetOrCreate(CacheKey.UserProfileImage, () => _userPageImage.GetAll()).ToList();

                if (userImages.Any())
                {
                    ProfileImage = userImages.FirstOrDefault(x => x.CreatedBy == User.Identity?.Name && x.Enabled == true)?.UploadedImage;
                }
            }

            var isUserRedirected = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (isUserRedirected == null)
            {
                return Page();
            }

            return await OnPostUserCommentSubmit();
        }

        public IActionResult OnPostUserReviewSubmit()
        {

            // TODO: Model Validation

            var userReview = new Review
            {
                CreatedDate = DateTime.UtcNow,
                ProductId = Product.Id,
                Rating = Review.Rating,
                UserReview = Review.UserReview,
                CreatedBy = "Test User",
                Pros = Review.Pros,
                Cons = Review.Cons,
                Email = Review.Email,
                FullName = Review.FullName
            };

            _reviewBaseStore.AddEntity(userReview, CacheKey.GetReviews, true);
            return RedirectToPage("/ProductDetail", new { id = Product.Id });
        }

        public async Task<IActionResult> OnPostUserCommentSubmit()
        {
            var userComment = new Comment
            {
                CreatedBy = "Test User",
                CreatedDate = DateTime.UtcNow,
                UserComment = UserComment.UserComment,
                ProductId = Product.Id
            };

            if (string.IsNullOrWhiteSpace(User.Identity?.Name))
            {
                _cacheService.Add(CacheKey.AddThisUsersComment, userComment, 10);
                return RedirectToPage("/Account/Login", new { area = "Identity", ReturnUrl = $"/ProductDetail/{Product.Id}" });
            }

            var isUserRedirected = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (isUserRedirected != null)
            {
                userComment = _cacheService.GetItem<Comment>(CacheKey.AddThisUsersComment);
            }

            var user = await _userManager.GetUserAsync(User);
            var firstName = User.Claims.Where(x => x.Type == ClaimTypes.Name).Skip(1).FirstOrDefault()?.Value;
            userComment.CreatedBy = User.Identity.Name;
            userComment.FullName = $"{firstName} {User.FindFirstValue(ClaimTypes.Surname)}";
            userComment.Email = user.Email;

            _commentBaseStore.AddEntity(userComment, CacheKey.GetComments, true);
            _cacheService.Remove(CacheKey.AddThisUsersComment);

            return Page();
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

        public class ImageData
        {
            public ImageData()
            {
                SecondaryImages = new List<string>();
            }

            public string PrimaryImage { get; set; } = string.Empty;

            public string PlaceHolderImage { get; set; } = string.Empty;

            public List<string> SecondaryImages { get; set; }
        }

        public class ReviewModel
        {
            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Full Name"), DataType(DataType.Text), RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
            public string FullName { get; set; }

            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Email"), DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Review"), DataType(DataType.Text)]
            public string UserReview { get; set; }

            public int Rating { get; set; }

            public string Pros { get; set; }

            public string Cons { get; set; }
        }

        public class UserCommentModel
        {
            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Full Name"), DataType(DataType.Text), RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
            public string FullName { get; set; }

            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Email"), DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required, StringLength(100, MinimumLength = 3), Display(Name = "Comment"), DataType(DataType.Text)]
            public string UserComment { get; set; }

            public int ProductId { get; set; }
        }
    }
}

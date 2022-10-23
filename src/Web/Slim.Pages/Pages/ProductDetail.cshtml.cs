using System.ComponentModel.DataAnnotations;
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

        private ILogger<ProductDetailModel> _logger;

        private ICacheService _cacheService;

        public List<ImageData> ImagesToShow { get; set; }

        public ProductDetailModel(
            IBaseStore<Product> productBaseStore,
            ILogger<ProductDetailModel> logger,
            ICacheService cacheService, IBaseStore<Review> reviewBaseStore, IBaseStore<Comment> commentBaseStore)
        {
            _productBaseStore = productBaseStore;
            _logger = logger;
            _cacheService = cacheService;
            _reviewBaseStore = reviewBaseStore;
            _commentBaseStore = commentBaseStore;

            ImagesToShow = new List<ImageData>();
            Product = new Product();
        }

        [BindProperty] public Product Product { get; set; }

        [BindProperty] public ReviewModel Review { get; set; } = new();
        [BindProperty] public UserCommentModel UserComment { get; set; } = new();


        public Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var product =
                _cacheService
                    .GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll)
                    .First(x => x.Id == id.Value);
            Product = product;
            return Task.FromResult<IActionResult>(Page());
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

        public IActionResult OnPostUserCommentSubmit()
        {
            // TODO: Model Validation

            var userComment = new Comment
            {
                CreatedBy = "Test User",
                CreatedDate = DateTime.UtcNow,
                FullName = UserComment.FullName,
                Email = UserComment.Email,
                UserComment = UserComment.UserComment,
                ProductId = Product.Id
            };

            _commentBaseStore.AddEntity(userComment, CacheKey.GetComments, true);

            return Page();
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

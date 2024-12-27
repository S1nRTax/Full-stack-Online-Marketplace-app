using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Net.Mime;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostController> _logger;

        public PostController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            ILogger<PostController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("create-post")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "Title cannot be empty or whitespace");
                    return BadRequest(ModelState);
                }

                if (model.Price < 0)
                {
                    ModelState.AddModelError("Price", "Price cannot be less than 0");
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = await _userManager.Users
                    .Include(u => u.Vendor)
                    .FirstOrDefaultAsync( u => u.Id == userId);


                if (currentUser == null)
                {
                    _logger.LogWarning("User not found while creating the post");
                    return Unauthorized();
                }

                if (!currentUser.HasShop || currentUser.Vendor == null)
                {
                    _logger.LogWarning("User {UserId} attempted to create post without vendor status", currentUser.Id);
                    return Unauthorized(new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Detail = "Only vendors can create posts",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }


                var post = new PostModel
                {
                    PostId = Guid.NewGuid().ToString(),
                    PostTitle = model.Title.Trim(),
                    PostImagePath = model.picture?.Trim(),
                    PostCreatedAt = DateTime.UtcNow,
                    PostPriceTag = model.Price,
                    PostUpVotes = 0,
                    VendorId = currentUser.Vendor.VendorId,
                    Vendor = currentUser.Vendor,
                };

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var response = new PostResponse
                    {
                        Id = post.PostId,
                        Title = post.PostTitle,
                        ImagePath = post.PostImagePath,
                        CreatedAt = post.PostCreatedAt,
                        Price = post.PostPriceTag,
                        UpVotes = post.PostUpVotes,
                        CreatedBy = currentUser.Vendor.ShopName ?? "Unknown",
                        CreatedById = currentUser.Vendor.VendorId
                    };

                    _logger.LogInformation("Post created successfully. PostId: {PostId}", post.PostId);

                    return CreatedAtAction(
                        nameof(GetPost),
                        new { id = post.PostId },
                        response
                    );
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Database error occurred while creating post");
                    return StatusCode(500, new ProblemDetails
                    {
                        Title = "Database Error",
                        Detail = "An error occurred while saving the post",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating post");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Server Error",
                    Detail = "An unexpected error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        // Update PostResponse to use decimal for Price
        public class PostResponse
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string? ImagePath { get; set; }
            public DateTime CreatedAt { get; set; }
            public double Price { get; set; }  // Changed from double to decimal
            public int UpVotes { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedById { get; set; }
        }
    

    [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "id" })]  // Cache for 1 minute
        public async Task<ActionResult<PostResponse>> GetPost(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid ID",
                    Detail = "Post ID cannot be empty",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            try
            {
                var post = await _context.Posts
                    .Include(p => p.Vendor)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PostId == id);


                if (post == null)
                {
                    _logger.LogWarning("Post not found. ID: {PostId}", id);
                    return NotFound(new ProblemDetails
                    {
                        Title = "Post not found",
                        Detail = "The requested Post does not exist",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var response = new PostResponse
                {
                    Id = post.PostId,
                    Title = post.PostTitle,
                    ImagePath = post.PostImagePath,
                    CreatedAt = post.PostCreatedAt,
                    Price = post.PostPriceTag,
                    UpVotes = post.PostUpVotes,
                    CreatedBy = post.Vendor?.ShopName ?? "Unknown",
                    CreatedById = post.Vendor?.ShopId ?? "Unknown",
                };

                _logger.LogInformation("Post retrieved successfully. PostId: {PostId}", id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retreiving post. PostId: {PostId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Server Error",
                    Detail = "An error occurred while retrieving the post",
                    Status = StatusCodes.Status500InternalServerError
                });
            }

        }



    }
}

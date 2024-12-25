using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;

        public PostController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            ITokenService tokenService,
            ILogger<AuthController> logger)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }


        [HttpPost("create-post")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model)
        {
            try
            {
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

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("User not found while creating the post");
                    return Unauthorized();
                }


                var post = new PostModel
                {
                    PostId = Guid.NewGuid().ToString(),
                    PostTitle = model.Title,
                    PostImagePath = model.picture,
                    PostCreatedAt = DateTime.UtcNow,
                    PostPriceTag = model.Price,
                    PostUpVotes = 0
                };

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {

                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Post created successfuly. PostId: " + post.PostId);

                    return CreatedAtAction(
                            nameof(GetPost),
                            new { id = post.PostId },
                            response
                        );
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Database error occured while creating the post");
                    return StatusCode(500, new ProblemDetails
                    {
                        Title = "Database error",
                        Detail = "An error occured while saving the post",
                        Status = 500
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured while creating the post");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Server error",
                    Detail = "An unexpected error occured while processing your request",
                    Status = 500
                });
            }
        }
        


                // response class: 
                public class PostResponse
                {
                    public string Id { get; set; }
                    public string Title { get; set; }
                    public string? ImagePath { get; set; }
                    public DateTime CreatedAt { get; set; }
                    public decimal Price { get; set; }
                    public int UpVotes { get; set; }
                    public string CreatedBy { get; set; }
                }

        [HttpGet]
        public async Task GetPost()
        {

   
        
        
        }




}

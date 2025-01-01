namespace Server.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IWebHostEnvironment _environment; // For accessing wwwroot folder
        private readonly ILogger<ProfilePictureService> _logger;
        private const string ProfilePictureFolder = "profile-pictures";
        private const string PostsPicturesFolder = "posts-pictures";


        public ProfilePictureService(IWebHostEnvironment environment, ILogger<ProfilePictureService> logger)
        {
            _environment = environment;
            _logger = logger;
        }


        public async Task<string> SaveProfilePictureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("invalid file.");


            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var folderPath = Path.Combine(_environment.WebRootPath, ProfilePictureFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/{ProfilePictureFolder}/{fileName}"; 
        }

        public async Task<string> SavePostPictureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            // Validate file size (10MB limit)
            if (file.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 10MB limit.");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, and .png files are allowed.");
            }

            try
            {
                var filename = Guid.NewGuid().ToString() + extension;
                var folderPath = Path.Combine(_environment.WebRootPath, PostsPicturesFolder);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{PostsPicturesFolder}/{filename}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving post picture");
                throw new Exception("Failed to save the image. Please try again.", ex);
            }
        }

        public void DeleteProfilePicture(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}

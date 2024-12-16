namespace Server.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IWebHostEnvironment _environment; // For accessing wwwroot folder
        private const string ProfilePictureFolder = "profile-pictures";


        public ProfilePictureService(IWebHostEnvironment environment)
        {
            _environment = environment;
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

            return $"/{ProfilePictureFolder}/{fileName}"; // this returns the relative path.
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

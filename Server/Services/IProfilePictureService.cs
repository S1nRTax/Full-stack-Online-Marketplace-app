namespace Server.Services
{
    public interface IProfilePictureService
    {
        Task<string> SaveProfilePictureAsync(IFormFile file);

        Task<string> SavePostPictureAsync(IFormFile file);
        void DeleteProfilePicture(string filePath);
    }
}

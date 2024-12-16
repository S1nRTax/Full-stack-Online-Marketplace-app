namespace Server.Services
{
    public interface IProfilePictureService
    {
        Task<string> SaveProfilePictureAsync(IFormFile file);
        void DeleteProfilePicture(string filePath);
    }
}

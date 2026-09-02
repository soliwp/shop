using System.Security.AccessControl;

namespace shop_file_upload_.common
{
    public interface IUploaderFile
    {
        Task<string> uploadFileAsync(IFormFile file, string destinationPath);
        void deleteFile(string path);
    }
}

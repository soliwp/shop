namespace shop_file_upload_.common
{
    public class UploaderFile : IUploaderFile
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public UploaderFile(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void deleteFile(string path)
        {
            var Destinationpath = Path.Combine(_hostEnvironment.WebRootPath, path);
            if (Path.Exists(Destinationpath)) 
            {
                File.Delete(Destinationpath);
            }
        }

        public async Task<string> uploadFileAsync(IFormFile file, string destinationPath)
        {
            const long MaxFileSize = 5 * 1024 * 1024; // 5MB
            var allowedExtensions = new[] { ".jpg" , ".jpeg" , ".png" , ".webp" };
            var allowedTypes= new[] { "image/jpg" , "image/png" , "image/webp" , "image/jpeg"};

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (file.Length >= MaxFileSize)
            {
                throw new Exception("اندازه فایل باید کوچک تر از 5 مگابایت باشد");
            }

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("فرمت فایل نامعتبر است");
            }

            if (!allowedTypes.Contains(file.ContentType))
            {
                throw new Exception("نوع فایل نا معتبر است");
            }


            var directory = Path.Combine(_hostEnvironment.WebRootPath, destinationPath);
            if (!Directory.Exists(directory)) 
            {
                Directory.CreateDirectory(directory);
            }
            var fileName = $"{Guid.NewGuid()} {Path.GetExtension(file.FileName)}";
            var path = Path.Combine(directory, fileName);

            var stream = new FileStream(path, FileMode.Create,FileAccess.Write,FileShare.None);
            await file.CopyToAsync(stream);

            return Path.Combine(destinationPath , fileName).Replace("\\" , "/");
        }
    }
}

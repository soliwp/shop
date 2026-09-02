namespace shop_file_upload_.common
{
    public class OperationResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public OperationResult Succeed(string message = "عملیات با موفقیت انجام شد") 
        {
            this.Message = message;
            Success = true;
            return this;
        }
        public OperationResult Failed (string message = "عملیات انجام نشد")
        {
            this.Message = message;
            Success=false;
            return this;
        }
    }
}

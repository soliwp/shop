using shop_file_upload_.common;

namespace shop_file_upload_.viewModel;
public class ProductImportViewModel
{
    public IFormFile UploadFile { get; set; }
    public List<importRowResult<ProductViewModel>> rows { get; set; } = new();
    public string message { get; set; } = string.Empty;
}

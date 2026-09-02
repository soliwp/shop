using shop_file_upload_.common;
using shop_file_upload_.viewModel;

namespace shop_file_upload_.services;
public interface IproductServices
{
    Task<ProductListViewModel> GetProductsAsync(ProductFillterModel model);
    Task<OperationResult> CreateProductAsync(ProductCreationView model);
    Task<ProductUpdateView> GetProductForUpdateAsync(int id);
    Task deteleProductImageAsync (int id);
    Task<OperationResult> UpdateProductAsync(ProductUpdateView model);
    OperationResult DeleteProduct(int id);
    Task<ProductViewModel> GetProductByIdAsync(int id);
    Task<byte[]> ExportProductExcelAsync(ProductFillterModel model);
    Task<ImportResult<ProductViewModel>> ImportAsync(IFormFile excelFile);
    Task<byte[]> downoadExcelTemplateFileAsync();
    Task<byte[]> generatePDFAsync(ProductFillterModel model);
}

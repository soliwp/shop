using Microsoft.AspNetCore.Http.Metadata;

namespace shop_file_upload_.viewModel;

public class ProductViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public int Price { get; set; }
    public string? MainImageURL { get; set; }
    public List<ProductImagesView> ProductImages { get; set; }
}

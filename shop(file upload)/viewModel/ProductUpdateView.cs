namespace shop_file_upload_.viewModel;

public class ProductUpdateView : ProductCreationView
{
    public int ProductId { get; set; }
    public string? ProductMainImageURL { get; set; }
    public int ProductCategoryId { get; set; }
    public List<ProductImagesView> ProductImages { get; set; } = new();
}
namespace shop_file_upload_.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int ProductPrice { get; set; }
    public bool IsDeleted { get; set; }    
    public string? ProductMainImageURL { get; set; }
    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public List<ProductImages> productImages { get; set; } = new();
}

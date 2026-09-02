namespace shop_file_upload_.Models;
public class ProductCategory
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public int? parentId { get; set; }
    public ProductCategory Parent {  get; set; }
    public List<ProductCategory> children { get; set; } = new();
    public List<Product> products { get; set; } = new();
}

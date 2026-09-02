namespace shop_file_upload_.viewModel;

public class ProductCreationView
{
    public string? Name { get; set; }
    public int? Price {  set; get; }
    public int CategoryId { get; set; }
    public IFormFile? MainImage { get; set; }
    public List<IFormFile?>? Images { get; set; }

}

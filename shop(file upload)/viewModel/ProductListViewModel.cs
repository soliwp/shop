namespace shop_file_upload_.viewModel;

public class ProductListViewModel : ProductFillterModel
{
    public List<ProductViewModel> Products { get; set; }
    public int totalPage { get; set; }
    public int currentPage { get; set; }
}
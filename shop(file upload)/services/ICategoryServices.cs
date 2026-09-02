using shop_file_upload_.Models;

namespace shop_file_upload_.services
{
    public interface ICategoryServices
    {
        Task<List<CategoryComboBoxModel>> GetCategoriesAsync();
    }
}

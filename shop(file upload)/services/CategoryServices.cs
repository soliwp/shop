using Microsoft.EntityFrameworkCore;
using shop_file_upload_.Models;

namespace shop_file_upload_.services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly shopDbContext _dbContext;

        public CategoryServices(shopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryComboBoxModel>> GetCategoriesAsync()
        {
            return await _dbContext.ProductCategories.Where(cat => !cat.children.Any()).Select(cat => new CategoryComboBoxModel
            {
                Id = cat.Id,
                Name = cat.CategoryName
            }).ToListAsync();
        }
    }
}

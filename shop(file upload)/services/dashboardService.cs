using Microsoft.EntityFrameworkCore;
using shop_file_upload_.viewModel;

namespace shop_file_upload_.services;

public class dashboardService : IdashboardService
{
    private readonly shopDbContext _context;

    public dashboardService(shopDbContext context)
    {
        _context = context;
    }

    public ProductRepoortChart ShowChart()
    {
        var product =  _context.Products.Where(p => !p.IsDeleted)
            .GroupBy(p => p.ProductCategory.CategoryName)
            .Select(p => new
            {
                name = p.Key,
                count = p.Count()
            })
            .ToList();
        var result = new ProductRepoortChart()
        {
            CategoriyName = product.Select(p => p.name).ToList(),
            ProductCount = product.Select(p => p.count).ToList(),
        };
        return result;
    }
}
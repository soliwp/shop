using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using shop_file_upload_.common;
using shop_file_upload_.common.PDF;
using shop_file_upload_.Models;
using shop_file_upload_.viewModel;

namespace shop_file_upload_.services;

public class ProductServices : IproductServices
{
    private readonly shopDbContext _shopDbContext;
    private readonly IUploaderFile _uploader;
    public readonly IConfiguration _configuration;
    public readonly IExcelService _excelService;
    private readonly IPDFService _PDFService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductServices(shopDbContext shopDbContext, IUploaderFile uploader, IConfiguration configuration, IExcelService excelService, IPDFService pDFService, IWebHostEnvironment webHostEnvironment)
    {
        _shopDbContext = shopDbContext;
        _uploader = uploader;
        _configuration = configuration;
        _excelService = excelService;        
        _PDFService = pDFService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<OperationResult> CreateProductAsync(ProductCreationView model)
    {
        var imagesPath = new List<string>();
        OperationResult result = new();
        if (await _shopDbContext.Products.AnyAsync(p => p.ProductName == model.Name))
        {
            return result.Failed("محصول تکراری است");
        }
        await using var transaction = await _shopDbContext.Database.BeginTransactionAsync();
        try
        {
            Product newProduct = new()
            {
                ProductName = model.Name,
                IsDeleted = false,
                ProductCategoryId = model.CategoryId,
                ProductPrice = Convert.ToInt32(model.Price)
            };

            if (model.MainImage != null)
            {
                var mainImagePath = await _uploader.uploadFileAsync(model.MainImage, "uploads/productImages");
                newProduct.ProductMainImageURL = mainImagePath;
                imagesPath.Add(mainImagePath);
            }
            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    var Images = await _uploader.uploadFileAsync(image, "uploads/productImages");
                    newProduct.productImages.Add(new ProductImages() { ImageURL = Images });
                    imagesPath.Add(Images);
                }
            }

            await _shopDbContext.Products.AddAsync(newProduct);
            await _shopDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return result.Succeed("محصول با موفقیت اضافه شد");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            foreach (var image in imagesPath)
            {
                _uploader.deleteFile(image);
            }
            return result.Failed("خطا در هنگام افزودن محصول رخ داده است");
        }
    }

    public async Task deteleProductImageAsync(int id)
    {
        var product = _shopDbContext.productImages.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            return;
        }
        else
        {
            _shopDbContext.productImages.Remove(product);
            await _shopDbContext.SaveChangesAsync();
            _uploader.deleteFile(product.ImageURL);
        }
    }

    public async Task<ProductViewModel> GetProductByIdAsync(int id)
    {
        var product = await _shopDbContext.Products.Include(p => p.productImages).Include(p => p.ProductCategory).FirstOrDefaultAsync(x => x.Id == id);
        var result = new ProductViewModel()
        {            
            Name= product.ProductName,
            Price = product.ProductPrice,
            MainImageURL= product.ProductMainImageURL,
            CategoryName= product.ProductCategory.CategoryName,
            ProductImages = product.productImages.Select(images => new ProductImagesView()
            {
                ImageId = images.Id,
                ImageURL = images.ImageURL,
                ProductId = id
            }).ToList(),
        };
        return result;
    }

    public async Task<ProductUpdateView> GetProductForUpdateAsync(int id)
    {
        var product = await _shopDbContext.Products.Include(p => p.productImages).FirstOrDefaultAsync(x => x.Id == id);
        var result = new ProductUpdateView()
        {
            ProductId = product.Id,
            Price = product.ProductPrice,
            ProductMainImageURL = product.ProductMainImageURL,
            ProductCategoryId = product.ProductCategoryId,
            Name = product.ProductName,
            ProductImages = product.productImages.Select(images => new ProductImagesView()
            {
                ImageId = images.Id,
                ImageURL = images.ImageURL,
                ProductId = id
            }).ToList(),
        };
        return result;
    }
    public IQueryable<Product> getFilteredProducts(ProductFillterModel model)
    {
        var query = _shopDbContext.Products.Include(p => p.ProductCategory).Where(p => p.IsDeleted == false).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(model.Name))
        {
            query = query.Where(p => p.ProductName.Contains(model.Name));
        }
        if (model.MinPrice > 0)
        {
            query = query.Where(p => p.ProductPrice >= model.MinPrice);
        }
        if (model.MaxPrice > 0)
        {
            query = query.Where(p => p.ProductPrice <= model.MaxPrice);
        }
        if (model.CategoryId > 0)
        {
            query = query.Where(p => p.ProductCategoryId == model.CategoryId);
        }
        return query;
    }
    public async Task<byte[]> ExportProductExcelAsync(ProductFillterModel model)
    {
        var query = await getFilteredProducts(model).Select(p => new ProductViewModel()
        {         
            ProductId = p.Id,
            Name= p.ProductName,
            Price= p.ProductPrice,
            CategoryName = p.ProductCategory.CategoryName
        }).ToListAsync();

        var columns = new List<Excelcolumns<ProductViewModel>>()
        {
            new Excelcolumns<ProductViewModel>()
            {
                columnTitle = "شناسه محصول",
                value = p => p.ProductId
            },
            new Excelcolumns<ProductViewModel>()
            {
                columnTitle = "نام محصول",
                value = p => p.Name
            },
            new Excelcolumns<ProductViewModel>()
            {
                columnTitle = "دسته بندی محصول",
                value = p => p.CategoryName
            },
            new Excelcolumns<ProductViewModel>()
            {
                columnTitle = "قیمت محصول",
                value = p => p.Price
            },
        };
        return await _excelService.ExportAsync(query,columns,"export");
    }
    public async Task<ProductListViewModel> GetProductsAsync(ProductFillterModel model)
    {
        var query = getFilteredProducts(model);
        //var query = _shopDbContext.Products.Include(p => p.ProductCategory).Where(p => p.IsDeleted == false).AsNoTracking().AsQueryable();

        //if (!string.IsNullOrEmpty(model.Name))
        //{
        //    query = query.Where(p => p.ProductName.Contains(model.Name));
        //}
        //if (model.MinPrice > 0)
        //{
        //    query = query.Where(p => p.ProductPrice >= model.MinPrice);
        //}
        //if (model.MaxPrice > 0)
        //{
        //    query = query.Where(p => p.ProductPrice <= model.MaxPrice);
        //}
        //if (model.CategoryId > 0)
        //{
        //    query = query.Where(p => p.ProductCategoryId == model.CategoryId);
        //}
        var pageSize = _configuration.GetValue<int>("pagination:pagesize");
        var totalProducts = await query.CountAsync();
        var totalPages = (int) Math.Ceiling((double) totalProducts / pageSize);
        model.pageNumber = model.pageNumber == 0 ? 1 : model.pageNumber;

        var product = await query.Select(p => new ProductViewModel
        {
            ProductId = p.Id,
            Name = p.ProductName,
            Price = p.ProductPrice,
            CategoryName = p.ProductCategory.CategoryName,
            MainImageURL = p.ProductMainImageURL,
            ProductImages = p.productImages.Select(image => new ProductImagesView()
            {
                ImageId = image.Id,
                ImageURL = image.ImageURL,
            }).ToList()
        }).Skip((model.pageNumber - 1)* pageSize).Take(pageSize).ToListAsync();

        ProductListViewModel result = new()
        {
            Products = product,
            CategoryId = model.CategoryId,
            MaxPrice = model.MaxPrice,
            MinPrice = model.MinPrice,
            Name = model.Name,            
            currentPage = model.pageNumber,
            totalPage = totalPages,            
        };
        return result;
    }

    public OperationResult DeleteProduct(int id)
    {
        OperationResult result = new();        
        try
        {
            var product =  _shopDbContext.Products.Include(p => p.productImages).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return result.Failed("محصول پیدا نشد");
            }
            _shopDbContext.Remove(product);
            _shopDbContext.SaveChangesAsync();
            
            if (product.ProductMainImageURL is not null)
            {
                _uploader.deleteFile(product.ProductMainImageURL);
            }
            if (product.productImages.Any())
            {
                foreach (var image in product.productImages) 
                {
                    _uploader.deleteFile(image.ImageURL);
                }
            }
            return result.Succeed();
        }
        catch (Exception)
        {
            return result.Failed();
        }
    }
    public async Task<OperationResult> UpdateProductAsync(ProductUpdateView model)
    {
        var imagesPath = new List<string>();
        OperationResult result = new();


        await using var transaction = await _shopDbContext.Database.BeginTransactionAsync();
        try
        {
            var OldProduct = await _shopDbContext.Products.FirstOrDefaultAsync(p => p.Id == model.ProductId);
            if (OldProduct is null)
            {
                return result.Failed("محصول پیدا نشد");
            }
            if (await _shopDbContext.Products.AnyAsync(p => p.ProductName == model.Name && p.Id != model.ProductId))
            {
                return result.Failed("محصول تکراری است");
            }
            OldProduct.ProductPrice = Convert.ToInt32(model.Price);
            OldProduct.ProductName = model.Name;
            OldProduct.ProductCategoryId = model.ProductCategoryId;
            var MainImagePath = OldProduct.ProductMainImageURL;

            if (model.MainImage != null)
            {
                OldProduct.ProductMainImageURL =await _uploader.uploadFileAsync(model.MainImage , "uploads/productImages");
                imagesPath.Add(OldProduct.ProductMainImageURL);
            }
            if (model.Images is not null)
            {
                foreach (var image in model.Images)
                {
                    var Path = await _uploader.uploadFileAsync(image, "uploads/productImages");
                    OldProduct.productImages.Add(new ProductImages()
                    {
                        ImageURL = Path,
                        ProductId = model.ProductId,
                    }); 
                    imagesPath.Add(Path);   
                }
            }

            await _shopDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            if (model.MainImage != null) 
            {
                _uploader.deleteFile(MainImagePath);
            }

            return result.Succeed();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            foreach(var image in imagesPath)
            {
                _uploader.deleteFile(image);
            }
            return result.Failed();
        }
    }

    public async Task<ImportResult<ProductViewModel>> ImportAsync(IFormFile excelFile)
    {
        var result = new ImportResult<ProductViewModel>();

        var excel = await _excelService.ReadExcelAsync(excelFile);
        foreach(var row in excel)
        {
            var rowResult = new importRowResult<ProductViewModel>()
            {
                rowNumber = row.rowNumber,
                Title = row.values[0]
            };
            try
            {
                string name = row.values[0];
                var product = await _shopDbContext.Products.FirstOrDefaultAsync(p => p.ProductName == name);
                if (product != null)
                {
                    rowResult.success = false;
                    rowResult.message = "محصول تکراری است";
                    result.rows.Add(rowResult);
                    result.failedRows++;
                    continue;
                }
                Product newproduct = new()
                {
                    ProductName = name,
                    IsDeleted = false,
                    ProductPrice = Convert.ToInt32(row.values[1]),
                    ProductCategoryId = Convert.ToInt32(row.values[2])
                };
                await _shopDbContext.Products.AddAsync(newproduct);

                rowResult.success = true;
                rowResult.message = "ثبت شد";
                rowResult.data = new ProductViewModel()
                {
                    Name = name,
                    Price = Convert.ToInt32(row.values[1]),
                };
                result.rows.Add(rowResult);
                result.SuccessRows++;
            }
            catch (Exception)
            {
                rowResult.success = false;
                rowResult.message = "ثبت نشد";
                result.failedRows++;
            }
        }
            await _shopDbContext.SaveChangesAsync();
            return result;
    }

    public async Task<byte[]> downoadExcelTemplateFileAsync()
    {
        ExcelPackage.License.SetNonCommercialPersonal("soliwp");
        using var excel = new ExcelPackage();
        var excelTemplate = excel.Workbook.Worksheets.Add("template");
        excelTemplate.Cells[1, 1].Value = "نام محصول";
        excelTemplate.Cells[1, 2].Value = "قیمت محصول";
        excelTemplate.Cells[1, 3].Value = "شماره دسته بندی محصول";

        excelTemplate.Cells[excelTemplate.Dimension.Address].AutoFitColumns();
        return await excel.GetAsByteArrayAsync();
    }

    public async Task<byte[]> generatePDFAsync(ProductFillterModel model)
    {
        var products = getFilteredProducts(model).Select(p => new ProductViewModel
        {
            ProductId = p.Id,
            Name = p.ProductName,
            Price = p.ProductPrice,
            CategoryName = p.ProductCategory.CategoryName
        });

        List<PDFColumn<ProductViewModel>> columns = new()
        {
            new PDFColumn<ProductViewModel>(){Header = "شناسه" , Value = p => p.ProductId.ToString()},
            new PDFColumn<ProductViewModel>(){Header = "نام محصول" , Value = p => p.Name},
            new PDFColumn<ProductViewModel>(){Header = "قیمت" , Value = p => p.Price.ToString("N0")},
            new PDFColumn<ProductViewModel>(){Header = "دسته بندی" , Value = p => p.CategoryName}
        };

        PDFOptions options = new()
        {
            Title = "محصولات",
            ShowFooter = true,
            ShowPageNumber = true,
            ShowPrintDate = true,
            logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Csharp_Logo.png")
        };
        return _PDFService.GeneratePDF<ProductViewModel>(products,columns,options);
    }

}
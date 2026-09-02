using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using shop_file_upload_.services;
using shop_file_upload_.viewModel;

namespace shop_file_upload_.Controllers
{
    public class ProductController : Controller
    {
        private readonly IproductServices _productServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IValidator<ProductCreationView> _ProductCreationValidator;

        public ProductController(IproductServices productServices, ICategoryServices categoryServices, IValidator<ProductCreationView> productCreationValidator)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
            _ProductCreationValidator = productCreationValidator;
        }
        public async Task<SelectList> showCategories()
        {
            var categories = await _categoryServices.GetCategoriesAsync();
            return new SelectList(categories, "Id", "Name");
        }
        public async Task<IActionResult> Index(ProductFillterModel filterModel)
        {
            var result = await _productServices.GetProductsAsync(filterModel);
            //var categories = await _categoryServices.GetCategoriesAsync();
            //ViewBag.categories = new SelectList(categories, "Id", "Name");
            ViewBag.categories = await showCategories();
            return View(result);
        }
        public async Task<IActionResult> Create()
        {
            //var categories = await _categoryServices.GetCategoriesAsync();
            //ViewBag.categories = new SelectList(categories, "Id", "Name");
            ViewBag.categories = await showCategories();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreationView model)
        {
            var validation = _ProductCreationValidator.Validate(model);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                //var categories = await _categoryServices.GetCategoriesAsync();
                //ViewBag.categories = new SelectList(categories, "Id", "Name");
                ViewBag.categories = await showCategories();
                return View(model);
            }
                var result = await _productServices.CreateProductAsync(model);
                if (result.Success)
                {
                    TempData["message"] = result.Message;
                    return RedirectToAction("index");
                }
                else
                {
                    //var categories = await _categoryServices.GetCategoriesAsync();
                    //ViewBag.categories = new SelectList(categories, "Id", "Name");
                    ViewBag.categories = await showCategories();
                    TempData["message"] = result.Message;
                    return View(model);
                }
            }

        public async Task<IActionResult> update(int id)
        {
            ViewBag.categories = await showCategories();
            //var categories = await _categoryServices.GetCategoriesAsync();
            //ViewBag.categories = new SelectList(categories, "Id", "Name");

            var model = await _productServices.GetProductForUpdateAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> update(ProductUpdateView model)
        {
            //var categories = await _categoryServices.GetCategoriesAsync();

            var validation = _ProductCreationValidator.Validate(model);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                ViewBag.categories = await showCategories();
                return await update(model.ProductId);
            }

            var result = await _productServices.UpdateProductAsync(model);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("index");
            }
            else
            {
                ViewBag.categories = await showCategories();
                TempData["message"] = result.Message;
                return RedirectToAction("update" , new {id = model.ProductId});
            }
        }
        public async Task<IActionResult> DeleteImage(int imageId , int productId) 
        {
            await _productServices.deteleProductImageAsync(imageId);
            return RedirectToAction("update" ,new { id = productId } );
        }
        public async Task<IActionResult> productDetails(int id) 
        {
            var product = await _productServices.GetProductByIdAsync(id);
            return View(product);
        }
        
        public IActionResult DeleteProduct(int productId)
        {
            var result = _productServices.DeleteProduct(productId);
            if (result.Success)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("index");
            }
            else
            {
                TempData["message"] = result.Message;
                return RedirectToAction("index");
            }
        }
        public async Task<IActionResult> excelExport(ProductFillterModel model)
        {
            var excelfile = await _productServices.ExportProductExcelAsync(model);
            return File(excelfile,"application/vnd.openXmlFormats-officedocument.spreadsheetml.sheet","product.xlsx");
        }
        public async Task<IActionResult> downloadExcelTemplate()
        {
            var excelfile = await _productServices.downoadExcelTemplateFileAsync();
            return File(excelfile, "application/vnd.openXmlFormats-officedocument.spreadsheetml.sheet", "Templateproduct.xlsx");
        }
        public IActionResult importProduct()
        {
            return View(new ProductImportViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> importProduct(ProductImportViewModel model)
        {
            var result = await _productServices.ImportAsync(model.UploadFile);
            model.rows = result.rows;
            model.message = $"{result.SuccessRows} تعداد ثبت موفق - {result.failedRows} تعداد ثبت ناموفق";
            return View(model);
        }
        public async Task<IActionResult> PDFExport(ProductFillterModel model)
        {
            var PDFFile = await _productServices.generatePDFAsync(model);
            return File(PDFFile, "application/pdf", "products.pdf");
        }
    }
}

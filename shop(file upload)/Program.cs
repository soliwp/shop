//using Asp.Versioning;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using shop_file_upload_;
using shop_file_upload_.common;
using shop_file_upload_.common.PDF;
using shop_file_upload_.services;
using shop_file_upload_.validations;
using shop_file_upload_.viewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IproductServices, ProductServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IUploaderFile, UploaderFile>();
builder.Services.AddScoped<IExcelService , ExcelService>();
builder.Services.AddScoped<IValidator<ProductCreationView>, productCreationValidation>();
builder.Services.AddScoped<IValidator<ProductUpdateView>, productUpdateValidation>();
builder.Services.AddScoped<IdashboardService , dashboardService>();

builder.Services.AddDbContext<shopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"));
});

// PDF maker
builder.Services.AddScoped<IPDFService, PDFService>();
var fontPath = Path.Combine(builder.Environment.WebRootPath, "fonts/BNazanin_0.ttf");
FontManager.RegisterFont(File.OpenRead(fontPath));
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// session 
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.SchemaName = "dbo";
    options.ConnectionString = builder.Configuration.GetConnectionString("connection");
    options.TableName = "session";
});
builder.Services.AddSession(sessionOption =>
{
    sessionOption.IOTimeout = TimeSpan.FromMinutes(30);
    sessionOption.Cookie.Name = "shopProject.session";
    sessionOption.Cookie.HttpOnly = true;
    sessionOption.Cookie.IsEssential = true;
    sessionOption.Cookie.SecurePolicy = CookieSecurePolicy.None;
    sessionOption.Cookie.SameSite = SameSiteMode.Lax;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using shop_file_upload_.viewModel;
namespace shop_file_upload_.validations;
public class productCreationValidation : AbstractValidator<ProductCreationView>
{
    public productCreationValidation()
    {
        string NotEmptyMessage = "این فیلد الزامی است";
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage(NotEmptyMessage)
            .Length(5, 50).WithMessage("نام نویسنده باید بین 5 تا 50 کاراکتر باشد");

        RuleFor(p => p.Price).NotNull().WithMessage(NotEmptyMessage)
            .GreaterThan(0).WithMessage("قیمت باید بزرگ تر از 0 باشد");

        RuleFor(p => p.MainImage)
            .Must(image => BeWithinMaxFileSize(image)).WithMessage("اندازه عکس باید کمتر از 1MB باشد")
            .Must(image => HaveAllowedExtension(image)).WithMessage("عکس باید یکی از فرمت های : jpg,jpeg,png و webp باشد");

        RuleFor(p => p.Images)
            .Must(BeWithinMaxFileSizeForImageList).WithMessage("اندازه عکس باید کمتر از 1MB باشد")
            .Must(HaveAllowedExtensionImageList).WithMessage("عکس باید یکی از فرمت های : jpg,jpeg,png و webp باشد");
      }
    private bool BeWithinMaxFileSize(IFormFile file)
    {
        if (file == null) return true;

        long maxSizeBytes = 1024 * 1024;
        return file.Length <= maxSizeBytes;
    }

    private bool HaveAllowedExtension(IFormFile file)
    {
        if (file == null) return true;

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }
    private bool BeWithinMaxFileSizeForImageList(List<IFormFile?>? file)
    {
        if (file == null) return true;
        return file.All(BeWithinMaxFileSize);
    }

    private bool HaveAllowedExtensionImageList(List<IFormFile?>? file)
    {
        if (file == null) return true;
        return file.All(HaveAllowedExtension);
    }
}

using FluentValidation;
using shop_file_upload_.viewModel;

namespace shop_file_upload_.validations;
public class productUpdateValidation : AbstractValidator<ProductUpdateView>
{
    public productUpdateValidation()
    {
        Include(new productCreationValidation());
    }
}
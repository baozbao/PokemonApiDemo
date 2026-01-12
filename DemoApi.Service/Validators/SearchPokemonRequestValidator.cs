using DemoApi.Service.Requests;
using FluentValidation;

namespace DemoApi.Service.Validators
{
    public class SearchPokemonRequestValidator : AbstractValidator<SearchPokemonRequest>
    {
        public SearchPokemonRequestValidator()
        {
            // Name: Optional, but if provided, max length 100
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            // Specie: Optional, max length 100
            RuleFor(x => x.Specie)
                .MaximumLength(100).WithMessage("Specie cannot exceed 100 characters.");

            // Type: Optional, max length 50
            RuleFor(x => x.Type)
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters.");

            // MinLevel: 1-100
            RuleFor(x => x.MinLevel)
                .InclusiveBetween(1, 100).WithMessage("MinLevel must be between 1 and 100.")
                .When(x => x.MinLevel.HasValue);

            // MaxLevel: 1-100
            RuleFor(x => x.MaxLevel)
                .InclusiveBetween(1, 100).WithMessage("MaxLevel must be between 1 and 100.")
                .When(x => x.MaxLevel.HasValue);

            // Cross-property: MaxLevel > MinLevel
            RuleFor(x => x.MaxLevel)
                .GreaterThanOrEqualTo(x => x.MinLevel).WithMessage("MaxLevel must be greater than or equal to MinLevel.")
                .When(x => x.MinLevel.HasValue && x.MaxLevel.HasValue);

            // Page: > 0
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");

            // PageSize: 1-999
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 999).WithMessage("PageSize must be between 1 and 999.");
        }
    }
}

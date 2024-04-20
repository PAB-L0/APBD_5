using FluentValidation;
using Labs_5.DTOs;

namespace Labs_5.Validators;

public class CreateAnimalRequestValidator : AbstractValidator<CreateAnimalRequest>
{
    public CreateAnimalRequestValidator()
    {
        RuleFor(createAnimalRequest => createAnimalRequest.Name).MaximumLength(200).NotNull();
        RuleFor(createAnimalRequest => createAnimalRequest.Description).MaximumLength(200);
        RuleFor(createAnimalRequest => createAnimalRequest.Category).MaximumLength(200).NotNull();
        RuleFor(createAnimalRequest => createAnimalRequest.Area).MaximumLength(200).NotNull();
    }    
}
using FluentValidation;
using Student_Management.DTO.GroupDto;

namespace Student_Management.Validations
{
    public class CreateGroupDtoValidator : AbstractValidator<CreateGoupDto>
    {
        public CreateGroupDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name boş ola bilməz.");
        }
    }
}
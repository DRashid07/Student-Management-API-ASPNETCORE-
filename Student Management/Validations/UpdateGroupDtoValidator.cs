using FluentValidation;
using Student_Management.DTO.GroupDto;

namespace Student_Management.Validations
{
    public class UpdateGroupDtoValidator : AbstractValidator<UpdateGroupDto>
    {
        public UpdateGroupDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name boş ola bilməz.");
        }
    }
}
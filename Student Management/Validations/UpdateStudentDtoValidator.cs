using FluentValidation;
using Student_Management.DTO.StudentDto;

namespace Student_Management.Validations
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName boş ola bilməz.")
                .MinimumLength(3).WithMessage("FullName minimum 3 simvol olmalıdır.");

            RuleFor(x => x.Age)
                .InclusiveBetween(16, 60).WithMessage("Age 16 və 60 arasında olmalıdır.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Email düzgün formatda olmalıdır.");
        }
    }
}
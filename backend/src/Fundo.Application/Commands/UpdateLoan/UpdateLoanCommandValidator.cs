using FluentValidation;
using Fundo.Domain.Enums;

namespace Fundo.Application.Commands.UpdateLoan
{
    internal sealed class UpdateLoanCommandValidator : AbstractValidator<UpdateLoanCommand>
    {
        public UpdateLoanCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Loan id is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.ApplicantName)
                .NotEmpty()
                .WithMessage("Applicant name is required.")
                .MaximumLength(200)
                .WithMessage("Applicant name must not exceed 200 characters.");

            RuleFor(x => x.CurrentBalance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Current balance cannot be negative.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be active or paid.");
        }
    }
}

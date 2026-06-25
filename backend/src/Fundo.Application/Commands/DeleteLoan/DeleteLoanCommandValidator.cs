using FluentValidation;

namespace Fundo.Application.Commands.DeleteLoan
{
    internal sealed class DeleteLoanCommandValidator : AbstractValidator<DeleteLoanCommand>
    {
        public DeleteLoanCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Loan id is required.");
        }
    }
}

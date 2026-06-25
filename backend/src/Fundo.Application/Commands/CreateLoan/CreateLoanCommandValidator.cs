using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Commands.CreateLoan
{
    internal sealed class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
    {
        public CreateLoanCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.ApplicantName)
                .NotEmpty()
                .WithMessage("Applicant name is required.")
                .MaximumLength(200)
                .WithMessage("Applicant name must not exceed 200 characters.");
        }
    }
}

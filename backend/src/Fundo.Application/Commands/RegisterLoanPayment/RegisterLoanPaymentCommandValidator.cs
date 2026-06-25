using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Commands.RegisterLoanPayment
{
    internal sealed class RegisterLoanPaymentCommandValidator : AbstractValidator<RegisterLoanPaymentCommand>
    {
        public RegisterLoanPaymentCommandValidator()
        {
            RuleFor(x => x.LoanId)
                .NotEmpty()
                .WithMessage("Loan id is required.");

            RuleFor(x => x.PaymentAmount)
                .GreaterThan(0)
                .WithMessage("Payment amount must be greater than zero.");
        }
    }
}

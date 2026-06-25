using Fundo.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Commands.RegisterLoanPayment
{
    public sealed record RegisterLoanPaymentCommand(
    Guid LoanId,
    decimal PaymentAmount) : ICommand;
}

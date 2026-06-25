using Fundo.Application.Abstractions;
using Fundo.Domain.Enums;
using System;

namespace Fundo.Application.Commands.UpdateLoan
{
    public sealed record UpdateLoanCommand(
        Guid Id,
        decimal Amount,
        string ApplicantName,
        decimal CurrentBalance,
        LoanStatus Status) : ICommand;
}

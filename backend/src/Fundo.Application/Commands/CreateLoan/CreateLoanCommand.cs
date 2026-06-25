using Fundo.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Commands.CreateLoan
{
    public sealed record CreateLoanCommand(
    decimal Amount,
    string ApplicantName) : ICommand<Guid>;

}

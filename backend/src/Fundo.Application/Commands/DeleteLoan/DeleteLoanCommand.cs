using Fundo.Application.Abstractions;
using System;

namespace Fundo.Application.Commands.DeleteLoan
{
    public sealed record DeleteLoanCommand(Guid Id) : ICommand;
}

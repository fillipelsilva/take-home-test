using Fundo.Application.Abstractions;
using Fundo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Queries.GetLoans
{
    public sealed record GetLoansQuery : IQuery<IReadOnlyList<LoanDto>>;
}

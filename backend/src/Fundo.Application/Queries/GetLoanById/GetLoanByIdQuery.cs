using Fundo.Application.Abstractions;
using Fundo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Queries.GetLoanById
{
    public sealed record GetLoanByIdQuery(Guid LoanId) : IQuery<LoanDto>;
}

using Fundo.Application.Abstractions;
using Fundo.Application.DTOs;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Application.Queries.GetLoans
{
    internal sealed class GetLoansQueryHandler : IQueryHandler<GetLoansQuery, IReadOnlyList<LoanDto>>
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoansQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result<IReadOnlyList<LoanDto>>> Handle(
            GetLoansQuery request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Loan> loans = await _loanRepository.GetAllAsync(cancellationToken);

            return Result.Success(LoanMapper.ToDtoList(loans));
        }
    }
}

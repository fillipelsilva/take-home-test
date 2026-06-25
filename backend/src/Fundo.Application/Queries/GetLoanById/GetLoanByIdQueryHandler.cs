using Fundo.Application.Abstractions;
using Fundo.Application.DTOs;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Fundo.Domain.Entities.Loan;

namespace Fundo.Application.Queries.GetLoanById
{
    internal sealed class GetLoanByIdQueryHandler : IQueryHandler<GetLoanByIdQuery, LoanDto>
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoanByIdQueryHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result<LoanDto>> Handle(
            GetLoanByIdQuery request,
            CancellationToken cancellationToken)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);

            if (loan is null)
            {
                return Result.Failure<LoanDto>(LoanErrors.NotFound);
            }

            return Result.Success(LoanMapper.ToDto(loan));
        }
    }
}

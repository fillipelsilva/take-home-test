using Fundo.Application.Abstractions;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Application.Commands.CreateLoan
{
    internal sealed class CreateLoanCommandHandler : ICommandHandler<CreateLoanCommand, Guid>
    {
        private readonly ILoanRepository _loanRepository;

        public CreateLoanCommandHandler(
            ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result<Guid>> Handle(
            CreateLoanCommand request,
            CancellationToken cancellationToken)
        {
            Loan loan = Loan.Create(
                request.Amount,
                request.ApplicantName);

            _loanRepository.Add(loan);

            await _loanRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(loan.Id);
        }
    }
}

using Fundo.Application.Abstractions;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using static Fundo.Domain.Entities.Loan;

namespace Fundo.Application.Commands.DeleteLoan
{
    internal sealed class DeleteLoanCommandHandler : ICommandHandler<DeleteLoanCommand>
    {
        private readonly ILoanRepository _loanRepository;

        public DeleteLoanCommandHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result> Handle(
            DeleteLoanCommand request,
            CancellationToken cancellationToken)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(request.Id, cancellationToken);

            if (loan is null)
            {
                return Result.Failure(LoanErrors.NotFound);
            }

            _loanRepository.Remove(loan);
            await _loanRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

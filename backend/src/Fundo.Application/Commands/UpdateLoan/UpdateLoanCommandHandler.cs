using Fundo.Application.Abstractions;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using static Fundo.Domain.Entities.Loan;

namespace Fundo.Application.Commands.UpdateLoan
{
    internal sealed class UpdateLoanCommandHandler : ICommandHandler<UpdateLoanCommand>
    {
        private readonly ILoanRepository _loanRepository;

        public UpdateLoanCommandHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result> Handle(
            UpdateLoanCommand request,
            CancellationToken cancellationToken)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(request.Id, cancellationToken);

            if (loan is null)
            {
                return Result.Failure(LoanErrors.NotFound);
            }

            Result updateResult = loan.UpdateDetails(
                request.Amount,
                request.ApplicantName,
                request.CurrentBalance,
                request.Status);

            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            await _loanRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

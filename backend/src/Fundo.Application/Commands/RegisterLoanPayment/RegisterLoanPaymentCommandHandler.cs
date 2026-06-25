using Fundo.Application.Abstractions;
using Fundo.Domain;
using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Fundo.Domain.Entities.Loan;

namespace Fundo.Application.Commands.RegisterLoanPayment
{
    internal sealed class RegisterLoanPaymentCommandHandler : ICommandHandler<RegisterLoanPaymentCommand>
    {
        private readonly ILoanRepository _loanRepository;

        public RegisterLoanPaymentCommandHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Result> Handle(
            RegisterLoanPaymentCommand request,
            CancellationToken cancellationToken)
        {
            Loan? loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);

            if (loan is null)
            {
                return Result.Failure(LoanErrors.NotFound);
            }

            Result paymentResult = loan.RegisterPayment(request.PaymentAmount);

            if (paymentResult.IsFailure)
            {
                return paymentResult;
            }

            await _loanRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

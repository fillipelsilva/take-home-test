using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fundo.Application.DTOs
{
    internal static class LoanMapper
    {
        public static LoanDto ToDto(Loan loan)
        {
            return new LoanDto(
                loan.Id,
                loan.Amount,
                loan.CurrentBalance,
                loan.ApplicantName,
                loan.Status);
        }

        public static IReadOnlyList<LoanDto> ToDtoList(IEnumerable<Loan> loans)
        {
            return loans.Select(ToDto).ToList();
        }
    }
}

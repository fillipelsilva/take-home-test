using Fundo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.DTOs
{
    public sealed record LoanDto(
    Guid Id,
    decimal Amount,
    decimal CurrentBalance,
    string ApplicantName,
    LoanStatus Status);
}

using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Application.Abstractions
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default);

        void Add(Loan loan);

        void Remove(Loan loan);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

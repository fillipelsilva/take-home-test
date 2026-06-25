using Fundo.Application.Abstractions;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Infrastructure.Repositories
{
    internal sealed class LoanRepository : ILoanRepository
    {
        private readonly LoanDbContext _dbContext;

        public LoanRepository(LoanDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Loans
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Loans
                .OrderByDescending(l => l.ApplicantName)
                .ToListAsync(cancellationToken);
        }

        public void Add(Loan loan)
        {
            _dbContext.Loans.Add(loan);
        }

        public void Remove(Loan loan)
        {
            _dbContext.Loans.Remove(loan);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

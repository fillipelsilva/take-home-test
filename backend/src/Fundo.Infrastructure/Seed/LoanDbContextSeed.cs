using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fundo.Infrastructure.Seed
{
    public static class LoanDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            LoanDbContext dbContext = scope.ServiceProvider.GetRequiredService<LoanDbContext>();
            ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("LoanDbContextSeed");

            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            if (await dbContext.Loans.AnyAsync())
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            Loan[] seedLoans = new[]
            {
            CreateSeedLoan(25000m, "John Doe", LoanStatus.Active),
            CreateSeedLoan(0m, "Jane Smith", LoanStatus.Paid),
            CreateSeedLoan(32500m, "Robert Johnson", LoanStatus.Active),
            CreateSeedLoan(5000m, "Diego Alves", LoanStatus.Paid),
            CreateSeedLoan(12000m, "Elena Rocha", LoanStatus.Paid)
        };

            dbContext.Loans.AddRange(seedLoans);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Seeded {Count} sample loans", seedLoans.Length);
        }

        private static Loan CreateSeedLoan(
            decimal amount,
            string applicantName,
            LoanStatus status)
        {
            Loan loan = Loan.Create(amount, applicantName);

            if (status == LoanStatus.Paid)
            {
                loan.RegisterPayment(amount);
            }

            return loan;
        }
    }
}

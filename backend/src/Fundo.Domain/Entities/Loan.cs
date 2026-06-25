using Fundo.Domain;
using Fundo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; private set; }

        public decimal Amount { get; private set; }

        public decimal CurrentBalance { get; private set; }

        public string ApplicantName { get; private set; } = string.Empty;

        public LoanStatus Status { get; private set; }

        private Loan()
        {
        }

        public static Loan Create(decimal amount, string applicantName)
        {
            return new Loan
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName,
                Status = LoanStatus.Active
            };
        }

        public Result RegisterPayment(decimal paymentAmount)
        {
            if (paymentAmount <= 0)
            {
                return Result.Failure(LoanErrors.InvalidPaymentAmount);
            }

            if (paymentAmount > CurrentBalance)
            {
                return Result.Failure(LoanErrors.PaymentExceedsBalance);
            }

            CurrentBalance -= paymentAmount;

            if (CurrentBalance == 0)
            {
                Status = LoanStatus.Paid;
            }

            return Result.Success();
        }

        public Result UpdateDetails(
            decimal amount,
            string applicantName,
            decimal currentBalance,
            LoanStatus status)
        {
            if (amount <= 0)
            {
                return Result.Failure(LoanErrors.InvalidAmount);
            }

            if (string.IsNullOrWhiteSpace(applicantName))
            {
                return Result.Failure(LoanErrors.ApplicantNameRequired);
            }

            if (currentBalance < 0)
            {
                return Result.Failure(LoanErrors.InvalidCurrentBalance);
            }

            if (currentBalance > amount)
            {
                return Result.Failure(LoanErrors.BalanceExceedsAmount);
            }

            if (status == LoanStatus.Paid && currentBalance > 0)
            {
                return Result.Failure(LoanErrors.PaidStatusRequiresZeroBalance);
            }

            if (status == LoanStatus.Active && currentBalance == 0)
            {
                return Result.Failure(LoanErrors.ActiveStatusRequiresPositiveBalance);
            }

            Amount = amount;
            ApplicantName = applicantName.Trim();
            CurrentBalance = currentBalance;
            Status = status;

            return Result.Success();
        }

        public static class LoanErrors
        {
            public static readonly Error NotFound = Error.NotFound(
                "Loan.NotFound",
                "The loan with the specified identifier was not found");

            public static readonly Error InvalidPaymentAmount = Error.Problem(
                "Loan.InvalidPaymentAmount",
                "Payment amount must be greater than zero");

            public static readonly Error PaymentExceedsBalance = Error.Problem(
                "Loan.PaymentExceedsBalance",
                "Payment amount cannot exceed current balance");

            public static readonly Error InvalidAmount = Error.Problem(
                "Loan.InvalidAmount",
                "Amount must be greater than zero");

            public static readonly Error ApplicantNameRequired = Error.Problem(
                "Loan.ApplicantNameRequired",
                "Applicant name is required");

            public static readonly Error InvalidCurrentBalance = Error.Problem(
                "Loan.InvalidCurrentBalance",
                "Current balance cannot be negative");

            public static readonly Error BalanceExceedsAmount = Error.Problem(
                "Loan.BalanceExceedsAmount",
                "Current balance cannot exceed the loan amount");

            public static readonly Error PaidStatusRequiresZeroBalance = Error.Problem(
                "Loan.PaidStatusRequiresZeroBalance",
                "Paid loans must have a zero current balance");

            public static readonly Error ActiveStatusRequiresPositiveBalance = Error.Problem(
                "Loan.ActiveStatusRequiresPositiveBalance",
                "Active loans must have a positive current balance");
        }
    }
}


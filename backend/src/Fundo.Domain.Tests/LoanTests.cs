using FluentAssertions;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Xunit;

namespace Fundo.Domain.Tests;

public class LoanTests
{
    [Fact]
    public void Create_ShouldInitializeWithActiveStatusAndFullBalance()
    {
        Loan loan = Loan.Create(1500m, "Maria Silva");

        loan.Amount.Should().Be(1500m);
        loan.CurrentBalance.Should().Be(1500m);
        loan.ApplicantName.Should().Be("Maria Silva");
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public void RegisterPayment_ShouldReduceBalance()
    {
        Loan loan = Loan.Create(1000m, "Joao Lima");

        var result = loan.RegisterPayment(400m);

        result.IsSuccess.Should().BeTrue();
        loan.CurrentBalance.Should().Be(600m);
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public void RegisterPayment_ShouldMarkAsPaid_WhenBalanceReachesZero()
    {
        Loan loan = Loan.Create(500m, "Ana Costa");

        var result = loan.RegisterPayment(500m);

        result.IsSuccess.Should().BeTrue();
        loan.CurrentBalance.Should().Be(0m);
        loan.Status.Should().Be(LoanStatus.Paid);
    }

    [Fact]
    public void RegisterPayment_ShouldFail_WhenPaymentExceedsBalance()
    {
        Loan loan = Loan.Create(300m, "Ricardo Nunes");

        var result = loan.RegisterPayment(500m);

        result.IsFailure.Should().BeTrue();
        loan.CurrentBalance.Should().Be(300m);
    }

    [Fact]
    public void RegisterPayment_ShouldFail_WhenAmountIsZeroOrNegative()
    {
        Loan loan = Loan.Create(300m, "Ricardo Nunes");

        loan.RegisterPayment(0m).IsFailure.Should().BeTrue();
        loan.RegisterPayment(-10m).IsFailure.Should().BeTrue();
    }
}

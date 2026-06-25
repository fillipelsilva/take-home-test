using FluentAssertions;
using Fundo.Application.DTOs;
using Fundo.Domain.Enums;
using Fundo.Services.Tests.Integration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration.Fundo.Applications.WebApi.Controllers;

public class LoanManagementControllerTests : IntegrationTestBase
{
    public LoanManagementControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoans_ShouldReturnSeededLoans_ForLoanManager()
    {
        HttpClient client = CreateAuthenticatedClient("LoanManager");

        HttpResponseMessage response = await client.GetAsync("/loans");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<LoanDto>? loans = await response.Content.ReadFromJsonAsync<List<LoanDto>>(JsonOptions);

        loans.Should().NotBeNull();
        loans!.Count.Should().BeGreaterThanOrEqualTo(5);
        loans.Should().Contain(l => l.ApplicantName == "John Doe");
    }

    [Fact]
    public async Task GetLoans_ShouldReturnSeededLoans_ForLoanAdmin()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage response = await client.GetAsync("/loans");

        response.EnsureSuccessStatusCode();

        List<LoanDto>? loans = await response.Content.ReadFromJsonAsync<List<LoanDto>>(JsonOptions);
        loans.Should().NotBeNull();
        loans!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLoans_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.GetAsync("/loans");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLoans_ShouldReturnForbidden_WhenUserHasNoLoanRole()
    {
        HttpClient client = CreateAuthenticatedClient("Guest");

        HttpResponseMessage response = await client.GetAsync("/loans");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetLoanById_ShouldReturnLoan_WhenExists()
    {
        HttpClient adminClient = CreateAuthenticatedClient("LoanAdmin");
        Guid loanId = await CreateLoanAsync(adminClient, 1500m, "Maria Silva");

        HttpClient readClient = CreateAuthenticatedClient("LoanManager");
        HttpResponseMessage response = await readClient.GetAsync($"/loans/{loanId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        LoanDto? loan = await response.Content.ReadFromJsonAsync<LoanDto>(JsonOptions);

        loan.Should().NotBeNull();
        loan!.Id.Should().Be(loanId);
        loan.Amount.Should().Be(1500m);
        loan.CurrentBalance.Should().Be(1500m);
        loan.ApplicantName.Should().Be("Maria Silva");
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public async Task GetLoanById_ShouldReturnNotFound_WhenLoanDoesNotExist()
    {
        HttpClient client = CreateAuthenticatedClient("LoanManager");

        HttpResponseMessage response = await client.GetAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetLoanById_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnCreated_WithInitialBalance()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount = 1000m,
            applicantName = "Maria Souza"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Guid loanId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);

        HttpResponseMessage getResponse = await client.GetAsync($"/loans/{loanId}");
        LoanDto? loan = await getResponse.Content.ReadFromJsonAsync<LoanDto>(JsonOptions);

        loan!.Amount.Should().Be(1000m);
        loan.CurrentBalance.Should().Be(1000m);
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnBadRequest_WhenAmountIsZero()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount = 0m,
            applicantName = "Maria Souza"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnBadRequest_WhenApplicantNameIsEmpty()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount = 1000m,
            applicantName = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount = 1000m,
            applicantName = "Maria Souza"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnForbidden_WhenUserHasReadOnlyRole()
    {
        HttpClient client = CreateAuthenticatedClient("LoanManager");

        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount = 1000m,
            applicantName = "Maria Souza"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReduceBalance()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        Guid loanId = await CreateLoanAsync(client, 1000m, "Joao Lima");

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{loanId}/payment",
            new { amount = 400m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        HttpResponseMessage getResponse = await client.GetAsync($"/loans/{loanId}");
        LoanDto? loan = await getResponse.Content.ReadFromJsonAsync<LoanDto>(JsonOptions);

        loan!.CurrentBalance.Should().Be(600m);
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public async Task RegisterPayment_ShouldMarkLoanAsPaid_WhenBalanceReachesZero()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        Guid loanId = await CreateLoanAsync(client, 500m, "Ana Costa");

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{loanId}/payment",
            new { amount = 500m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        HttpResponseMessage getResponse = await client.GetAsync($"/loans/{loanId}");
        LoanDto? loan = await getResponse.Content.ReadFromJsonAsync<LoanDto>(JsonOptions);

        loan!.CurrentBalance.Should().Be(0m);
        loan.Status.Should().Be(LoanStatus.Paid);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReturnBadRequest_WhenPaymentExceedsBalance()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        Guid loanId = await CreateLoanAsync(client, 300m, "Ricardo Nunes");

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{loanId}/payment",
            new { amount = 500m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReturnBadRequest_WhenAmountIsZero()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        Guid loanId = await CreateLoanAsync(client, 300m, "Ricardo Nunes");

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{loanId}/payment",
            new { amount = 0m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReturnNotFound_WhenLoanDoesNotExist()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{Guid.NewGuid()}/payment",
            new { amount = 100m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            $"/loans/{Guid.NewGuid()}/payment",
            new { amount = 100m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterPayment_ShouldReturnForbidden_WhenUserHasReadOnlyRole()
    {
        HttpClient adminClient = CreateAuthenticatedClient("LoanAdmin");
        Guid loanId = await CreateLoanAsync(adminClient, 1000m, "Joao Lima");

        HttpClient readOnlyClient = CreateAuthenticatedClient("LoanManager");

        HttpResponseMessage paymentResponse = await readOnlyClient.PostAsJsonAsync(
            $"/loans/{loanId}/payment",
            new { amount = 100m });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnNoContent_WhenLoanExists()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");
        Guid loanId = await CreateLoanAsync(client, 1000m, "Loan To Delete");

        HttpResponseMessage response = await client.DeleteAsync($"/loans/{loanId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        HttpResponseMessage getResponse = await client.GetAsync($"/loans/{loanId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnNotFound_WhenLoanDoesNotExist()
    {
        HttpClient client = CreateAuthenticatedClient("LoanAdmin");

        HttpResponseMessage response = await client.DeleteAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnForbidden_WhenUserHasReadOnlyRole()
    {
        HttpClient adminClient = CreateAuthenticatedClient("LoanAdmin");
        Guid loanId = await CreateLoanAsync(adminClient, 500m, "Protected Loan");

        HttpClient readOnlyClient = CreateAuthenticatedClient("LoanManager");

        HttpResponseMessage response = await readOnlyClient.DeleteAsync($"/loans/{loanId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.DeleteAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

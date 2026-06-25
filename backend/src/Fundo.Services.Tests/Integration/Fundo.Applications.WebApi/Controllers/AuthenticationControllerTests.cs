using FluentAssertions;
using Fundo.Application.DTOs;
using Fundo.Services.Tests.Integration;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration.Fundo.Applications.WebApi.Controllers;

public class AuthenticationControllerTests : IntegrationTestBase
{
    public AuthenticationControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateToken_ShouldReturnAccessToken()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/auth/token", new
        {
            username = "demo-user",
            roles = new List<string> { "LoanAdmin" }
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        TokenResponse? body = await response.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions);

        body.Should().NotBeNull();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GenerateToken_ShouldAllowLoanReadAccess_WithLoanManagerRole()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage tokenResponse = await client.PostAsJsonAsync("/auth/token", new
        {
            username = "manager-user",
            roles = new List<string> { "LoanManager" }
        });

        tokenResponse.EnsureSuccessStatusCode();

        TokenResponse? tokenBody = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenBody!.AccessToken);

        HttpResponseMessage loansResponse = await client.GetAsync("/loans");

        loansResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        List<LoanDto>? loans = await loansResponse.Content.ReadFromJsonAsync<List<LoanDto>>(JsonOptions);
        loans.Should().NotBeNull();
        loans!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GenerateToken_ShouldAllowLoanWriteAccess_WithLoanAdminRole()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage tokenResponse = await client.PostAsJsonAsync("/auth/token", new
        {
            username = "admin-user",
            roles = new List<string> { "LoanAdmin" }
        });

        tokenResponse.EnsureSuccessStatusCode();

        TokenResponse? tokenBody = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenBody!.AccessToken);

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/loans", new
        {
            amount = 750m,
            applicantName = "Token User"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private sealed record TokenResponse(string AccessToken);
}

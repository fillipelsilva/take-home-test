using Fundo.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    protected CustomWebApplicationFactory Factory { get; }

    protected IJwtTokenGenerator JwtTokenGenerator { get; }

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        JwtTokenGenerator = factory.Services.GetRequiredService<IJwtTokenGenerator>();
    }

    protected HttpClient CreateClient() =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    protected HttpClient CreateAuthenticatedClient(params string[] roles)
    {
        HttpClient client = CreateClient();
        string token = JwtTokenGenerator.GenerateToken("integration-test-user", roles);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected async Task<Guid> CreateLoanAsync(
        HttpClient client,
        decimal amount,
        string applicantName)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/loans", new
        {
            amount,
            applicantName
        });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
    }
}

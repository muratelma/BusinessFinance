using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Authentication;

namespace BusinessFinance.Api.Tests.Features.FinancePages;

public sealed class TransferFeeEndpointTests
{
    [Fact]
    public async Task FeeNeedsAnExplicitScopeWhenTheBusinessSourceHasNoLabelAndCancelsWithItsTransfer()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        const string password = "Valid-Synthetic-2026!";
        var email = $"fee-{Guid.NewGuid():N}@example.test";
        (await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, password, HasBusiness: true))).EnsureSuccessStatusCode();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, password));
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        async Task<Guid> Account(string name)
        {
            var response = await client.PostAsJsonAsync("/api/v1/accounts", new { name, type = "bank", currency = "TRY", openingBalance = "100" });
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        }
        var source = await Account("Çıkan"); var destination = await Account("Giren");
        object Request(string? scope, string date = "2026-08-01") => new
        { sourceAccountId = source, destinationAccountId = destination, amount = "10.1234", feeAmount = "1.2345", feeScope = scope, currency = "TRY", transferDate = date };
        var preview = await client.PostAsJsonAsync("/api/v1/transfers/preview", Request(null));
        preview.EnsureSuccessStatusCode();
        var p = await preview.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(p.GetProperty("needsScope").GetBoolean());
        Assert.Equal("11.3579", p.GetProperty("totalOutflow").GetString());
        var missing = await client.PostAsJsonAsync("/api/v1/transfers", Request(null));
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        var future = await client.PostAsJsonAsync("/api/v1/transfers", Request("business", "2099-01-01"));
        Assert.Equal(HttpStatusCode.BadRequest, future.StatusCode);
        var create = await client.PostAsJsonAsync("/api/v1/transfers", Request("personal"));
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid(); var fee = created.GetProperty("feeTransactionId").GetGuid();
        Assert.Equal("personal", created.GetProperty("feeScope").GetString());
        var parts = await client.GetFromJsonAsync<JsonElement>($"/api/v1/financial-activities/transfer/{id}/parts");
        Assert.Equal("fee", parts.GetProperty("items")[0].GetProperty("kind").GetString());
        Assert.Equal("1.2345", parts.GetProperty("items")[0].GetProperty("amount").GetString());
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync($"/api/v1/transactions/{fee}")).StatusCode);
        (await client.DeleteAsync($"/api/v1/transfers/{id}")).EnsureSuccessStatusCode();
        var cancelledFee = await client.GetFromJsonAsync<JsonElement>($"/api/v1/transactions/{fee}");
        Assert.True(cancelledFee.GetProperty("isCancelled").GetBoolean());
    }
}

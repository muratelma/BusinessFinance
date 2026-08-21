using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Application.Accounts.CreateAccount;
using BusinessFinance.Application.Accounts.DeactivateAccount;
using BusinessFinance.Application.Accounts.GetAccount;
using BusinessFinance.Application.Accounts.ListAccounts;
using BusinessFinance.Application.Authentication.LoginUser;
using BusinessFinance.Application.Authentication.Logout;
using BusinessFinance.Application.Authentication.RefreshTokens;
using BusinessFinance.Application.Authentication.RegisterUser;
using BusinessFinance.Application.Receipts;

namespace BusinessFinance.Api.Tests.Extensions;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationUseCases_RegistersEveryUseCaseAsTransient()
    {
        var services = new ServiceCollection();

        services.AddApplicationUseCases();

        Type[] expectedUseCases =
        [
            typeof(RegisterUserUseCase),
            typeof(LoginUserUseCase),
            typeof(RefreshTokensUseCase),
            typeof(LogoutUseCase),
            typeof(CreateAccountUseCase),
            typeof(ListAccountsUseCase),
            typeof(GetAccountUseCase),
            typeof(DeactivateAccountUseCase),
            typeof(AnalyzeReceiptUseCase)
        ];

        foreach (var useCase in expectedUseCases)
        {
            var descriptor = Assert.Single(
                services,
                service => service.ServiceType == useCase);

            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
            Assert.Equal(useCase, descriptor.ImplementationType);
        }
    }
}

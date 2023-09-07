using Domain.Interfaces;
using Infrastructure.BankCommunication;
using Infrastructure.Repository;
using MockBank;

namespace API.Installers;

public static class MockBankInstaller
{
    public static void AddMockBank(this IServiceCollection services)
    {
        services.AddSingleton<IMockBankClient, MockBankClient>();
    }
}
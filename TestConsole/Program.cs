// See https://aka.ms/new-console-template for more information

using Keyfactor.Extensions.Pam.AzureKeyVault;

namespace TestConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        KeyVaultPam pam = new();
        Dictionary<string, string> initializationInfo = new();
        Dictionary<string, string> instanceParameters = new();
        
        initializationInfo.Add("KeyVaultUri", Environment.GetEnvironmentVariable("KEYVAULT_VAULT_URI") ?? string.Empty);
        instanceParameters.Add("SecretId", Environment.GetEnvironmentVariable("KEYVAULT_SECRET_ID") ?? string.Empty);

        string password = pam.GetPassword(instanceParameters, initializationInfo);
        Console.WriteLine($"Password from Azure KeyVault: {password}");
    }
}
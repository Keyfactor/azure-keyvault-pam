// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
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
        
        Console.WriteLine($"Getting password from Azure Key Vault...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        string password = pam.GetPassword(instanceParameters, initializationInfo);
        stopwatch.Stop();
        
        Console.WriteLine($"Password from Azure Key Vault: {password}");
        Console.WriteLine($"Total elapsed time: {stopwatch.ElapsedMilliseconds}ms.");
    }
}
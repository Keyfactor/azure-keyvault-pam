// Copyright 2023 Keyfactor
// // 
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// // 
// //     http://www.apache.org/licenses/LICENSE-2.0
// // 
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.

using System.Diagnostics;
using Keyfactor.Extensions.Pam.AzureKeyVault;

namespace TestConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        TestAzureKeyVaultPam();
        TestAzureKeyVaultPamServicePrincipal();
    }

    private static void TestAzureKeyVaultPam()
    {
        Console.WriteLine("Testing Azure Key Vault PAM provider...");
        
        KeyVaultPam pam = new();
        Dictionary<string, string> initializationInfo = new();
        Dictionary<string, string> instanceParameters = new();
        
        initializationInfo.Add("KeyVaultUri", Environment.GetEnvironmentVariable("KEYVAULT_VAULT_URI") ?? string.Empty);
        instanceParameters.Add("SecretId", Environment.GetEnvironmentVariable("KEYVAULT_SECRET_ID") ?? string.Empty);
        instanceParameters.Add("AuthorityHost", Environment.GetEnvironmentVariable("AZURE_AUTHORITY_HOST") ?? "public");
        
        Console.WriteLine("Getting password from Azure Key Vault...");
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        string password = pam.GetPassword(instanceParameters, initializationInfo);
        
        stopwatch.Stop();
        
        Console.WriteLine($"Password from Azure Key Vault: {password}");
        Console.WriteLine($"Total elapsed time: {stopwatch.ElapsedMilliseconds}ms.");

        Console.WriteLine("Finished testing Azure Key Vault PAM provider.");
    }
    
    private static void TestAzureKeyVaultPamServicePrincipal()
    {
        Console.WriteLine("Testing Azure Key Vault Service Principal PAM provider...");
        
        KeyVaultPamServicePrincipal pam = new();
        Dictionary<string, string> initializationInfo = new();
        Dictionary<string, string> instanceParameters = new();
        
        initializationInfo.Add("KeyVaultUri", Environment.GetEnvironmentVariable("KEYVAULT_VAULT_URI") ?? string.Empty);
        instanceParameters.Add("SecretId", Environment.GetEnvironmentVariable("KEYVAULT_SECRET_ID") ?? string.Empty);
        instanceParameters.Add("AuthorityHost", Environment.GetEnvironmentVariable("AZURE_AUTHORITY_HOST") ?? "public");
        
        instanceParameters.Add("ClientId", Environment.GetEnvironmentVariable("AZURE_CLIENT_ID") ?? "bogus");
        instanceParameters.Add("ClientSecret", Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET") ?? "value");
        instanceParameters.Add("TenantId", Environment.GetEnvironmentVariable("AZURE_TENANT_ID") ?? "changeme");
        
        Console.WriteLine("Getting password from Azure Key Vault...");
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        string password = pam.GetPassword(instanceParameters, initializationInfo);
        
        stopwatch.Stop();
        
        Console.WriteLine($"Password from Azure Key Vault: {password}");
        Console.WriteLine($"Total elapsed time: {stopwatch.ElapsedMilliseconds}ms.");

        Console.WriteLine("Finished testing Azure Key Vault Service Principal PAM provider.");
    }
}

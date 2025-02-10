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
        KeyVaultPam pam = new();
        Dictionary<string, string> initializationInfo = new();
        Dictionary<string, string> instanceParameters = new();
        
        initializationInfo.Add("KeyVaultUri", Environment.GetEnvironmentVariable("KEYVAULT_VAULT_URI") ?? string.Empty);
        instanceParameters.Add("SecretId", Environment.GetEnvironmentVariable("KEYVAULT_SECRET_ID") ?? string.Empty);
        
        Console.WriteLine("Getting password from Azure Key Vault...");
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        string password = pam.GetPassword(instanceParameters, initializationInfo);
        
        stopwatch.Stop();
        
        Console.WriteLine($"Password from Azure Key Vault: {password}");
        Console.WriteLine($"Total elapsed time: {stopwatch.ElapsedMilliseconds}ms.");
    }
}

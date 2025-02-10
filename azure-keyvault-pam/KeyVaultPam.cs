// Copyright 2023 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    public class KeyVaultPam: IPAMProvider
    {
        public string Name => "Azure-KeyVault";
        private readonly ILogger _logger = LogHandler.GetClassLogger<KeyVaultPam>();
        private SecretClient _secretClient;

        public KeyVaultPam()
        {
        }

        public KeyVaultPam(SecretClient client)
        {
            _secretClient = client;
        }
        
        /// <summary>
        /// Retrieves a password value from Azure Key Vault.
        /// </summary>
        /// <param name="instanceParameters">A dictionary of strings for instanceParameters. This dictionary must
        /// contain a value for key `SecretId`.</param>
        /// <param name="initializationInfo">A dictionary of strings for initializationInfo. If the class default constructor
        /// is used, this dictionary must contain a value for `KeyVaultUri`.</param>
        /// <exception cref="KeyVaultPamException">Thrown when a required dictionary value is missing.</exception>
        /// <returns>A password from Azure Key Vault.</returns>
        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            string password = GetAzureKeyVaultSecret(instanceParameters, initializationInfo)
                .GetAwaiter()
                .GetResult();
            
            _logger.MethodExit(LogLevel.Debug);
            
            return password;
        }

        private async Task<string> GetAzureKeyVaultSecret(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);

            if (_secretClient is null)
            {
                _secretClient = GetSecretClient(initializationInfo);
            }
            
            
            string secretId = GetValueFromDictionary(instanceParameters, "instanceParameters", "SecretId");
            _logger.LogDebug($"SecretId: {secretId}");
            
            _logger.LogDebug("Getting secret from Azure Key Vault...");
            
            KeyVaultSecret secret = await _secretClient
                .GetSecretAsync(secretId)
                .ConfigureAwait(false);
            
            _logger.LogDebug("Finished getting secret from Azure Key Vault.");
            
            _logger.MethodExit(LogLevel.Debug);
            
            return secret.Value;
        }

        private SecretClient GetSecretClient(Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            _logger.LogDebug("Instantiating new instance of SecretClient...");
            
            string keyVaultUri = GetValueFromDictionary(initializationInfo, "initializationInfo", "KeyVaultUri");
            _logger.LogDebug($"KeyVaultUri: {keyVaultUri}");
            
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                AuthorityHost = GetAzureAuthorityHost()
            };
            
            SecretClient secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(options));
            
            _logger.LogDebug("Finished instantiating SecretClient.");
            
            _logger.MethodExit(LogLevel.Debug);
            
            return secretClient;
        }

        private Uri GetAzureAuthorityHost()
        {
            _logger.MethodEntry(LogLevel.Debug);
            string authorityHost = Environment.GetEnvironmentVariable("AZURE_AUTHORITY_HOST");
            
            if (string.IsNullOrWhiteSpace(authorityHost))
            {
                _logger.LogDebug("Using default Azure authority host.");
                return null;
            }
            
            _logger.LogDebug($"Authority Host: {authorityHost}");

            Uri resolvedAuthorityHost;
            
            switch (authorityHost)
            {
                case "china":
                    _logger.LogDebug("Using China Azure authority host.");
                    resolvedAuthorityHost = AzureAuthorityHosts.AzureChina;
                    break;
                case "government":
                    _logger.LogDebug("Using government Azure authority host.");
                    resolvedAuthorityHost = AzureAuthorityHosts.AzureGovernment;
                    break;
                case "public":
                    _logger.LogDebug("Using public Azure authority host.");
                    resolvedAuthorityHost = AzureAuthorityHosts.AzurePublicCloud;
                    break;
                default:
                    _logger.LogDebug("Using custom authority host.");
                    resolvedAuthorityHost = new Uri(authorityHost);
                    break;
            }
            
            _logger.MethodExit(LogLevel.Debug);

            return resolvedAuthorityHost;
        }

        private string GetValueFromDictionary(Dictionary<string, string> dictionary, string dictionaryName, string key)
        {
            _logger.MethodEntry(LogLevel.Debug);
            _logger.LogDebug($"Getting value of key {key} from dictionary {dictionaryName}...");
            
            if (!dictionary.ContainsKey(key) || string.IsNullOrEmpty(dictionary[key]))
            {
                string message = $"Dictionary {dictionaryName} is missing a value for {key}";
                _logger.LogError(message);
                throw new KeyVaultPamException(message);
            }
            
            string value = dictionary[key];
            _logger.MethodExit(LogLevel.Debug);
            _logger.LogDebug($"Finished getting value of key {key} from dictionary {dictionaryName}.");

            return value;
        }
    }
}
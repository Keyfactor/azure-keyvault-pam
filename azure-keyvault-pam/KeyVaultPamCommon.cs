// Copyright 2025 Keyfactor
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    public abstract class KeyVaultPamCommon: IPAMProvider
    {
        private readonly ILogger _logger;
        private SecretClient _secretClient;
        private readonly KeyVaultPamValueResolver _resolver;
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultPamCommon class with a logger.
        /// </summary>
        /// <param name="logger">Logger for diagnostic output</param>
        protected KeyVaultPamCommon(ILogger logger)
        {
            _logger = logger;
            _resolver = new KeyVaultPamValueResolver(logger);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultPamCommon class with a logger and pre-configured SecretClient.
        /// </summary>
        /// <param name="logger">Logger for diagnostic output</param>
        /// <param name="client">Pre-configured Azure Key Vault SecretClient</param>
        protected KeyVaultPamCommon(ILogger logger, SecretClient client)
        {
            _logger = logger;
            _secretClient = client;
            _resolver = new KeyVaultPamValueResolver(logger);
        }
        
        /// <summary>
        /// Synchronously retrieves a password value from Azure Key Vault.
        /// </summary>
        /// <param name="instanceParameters"> Dictionary containing instance-specific parameters</param>
        /// <param name="initializationInfo">Dictionary containing initialization parameters</param>
        /// <exception cref="KeyVaultPamException">Thrown when a required values are missing</exception>
        /// <returns>A retrieved secret value</returns>
        public string GetPassword(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry();

            string password = GetAzureKeyVaultSecret(instanceParameters, initializationInfo)
                .GetAwaiter()
                .GetResult();

            _logger.MethodExit();
            
            _logger.LogInformation("Successfully retrieved secret from Azure Key Vault.");

            return password;
        }

        /// <summary>
        /// The name identifier for the PAM provider implementation.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Internal method to retrieve a secret from Azure Key Vault.
        /// </summary>
        /// <param name="instanceParameters">Dictionary containing the SecretId</param>
        /// <param name="initializationInfo">Dictionary containing KeyVault configuration</param>
        /// <returns>The secret value from Azure Key Vault</returns>
        /// <exception cref="KeyVaultPamException">
        /// Thrown when:
        /// - Required configuration is missing
        /// - KeyVaultUri is invalid
        /// - KeyVault URI does not match expected format
        /// </exception>
        /// <exception cref="Azure.RequestFailedException">Thrown when the Azure Key Vault request fails</exception>
        private async Task<string> GetAzureKeyVaultSecret(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry();
            
            _logger.LogDebug($"Using PAM Provider: {Name}");

            if (_secretClient is null)
            {
                _secretClient = new KeyVaultClientFactory(_logger, initializationInfo).Create();
            }


            string secretId = _resolver.GetValueFromDictionary(instanceParameters, "instanceParameters", "SecretId");
            _logger.LogDebug($"SecretId: {secretId}");

            _logger.LogTrace("Getting secret from Azure Key Vault...");

            KeyVaultSecret secret = await _secretClient
                .GetSecretAsync(secretId)
                .ConfigureAwait(false);

            _logger.LogTrace("Finished getting secret from Azure Key Vault.");

            _logger.MethodExit();

            return secret.Value;
        }
    }
}

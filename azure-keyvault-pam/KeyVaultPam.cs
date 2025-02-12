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

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    public class KeyVaultPam : IPAMProvider
    {
        private readonly ILogger _logger = LogHandler.GetClassLogger<KeyVaultPam>();
        private readonly KeyVaultPamValueResolver _resolver;
        private SecretClient _secretClient;

        public KeyVaultPam()
        {
            _resolver = new KeyVaultPamValueResolver(_logger);
        }

        public KeyVaultPam(SecretClient client)
        {
            _secretClient = client;
            _resolver = new KeyVaultPamValueResolver(_logger);
        }

        public string Name => "Azure-KeyVault";

        /// <summary>
        ///     Retrieves a password value from Azure Key Vault.
        /// </summary>
        /// <param name="instanceParameters">
        ///     A dictionary of strings for instanceParameters. This dictionary must
        ///     contain a value for key `SecretId`.
        /// </param>
        /// <param name="initializationInfo">
        ///     A dictionary of strings for initializationInfo. If the class default constructor
        ///     is used, this dictionary must contain a value for `KeyVaultUri`.
        /// </param>
        /// <exception cref="KeyVaultPamException">Thrown when a required dictionary value is missing.</exception>
        /// <returns>A password from Azure Key Vault.</returns>
        public string GetPassword(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
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
                _secretClient = new KeyVaultClientFactory(_logger, initializationInfo).Create();
            }


            string secretId = _resolver.GetValueFromDictionary(instanceParameters, "instanceParameters", "SecretId");
            _logger.LogDebug($"SecretId: {secretId}");

            _logger.LogDebug("Getting secret from Azure Key Vault...");

            KeyVaultSecret secret = await _secretClient
                .GetSecretAsync(secretId)
                .ConfigureAwait(false);

            _logger.LogDebug("Finished getting secret from Azure Key Vault.");

            _logger.MethodExit(LogLevel.Debug);

            return secret.Value;
        }
    }
}

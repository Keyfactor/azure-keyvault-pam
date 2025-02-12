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
using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    public class KeyVaultClientFactory
    {
        private readonly ILogger _logger;
        private readonly KeyVaultPamValueResolver _resolver;

        public KeyVaultClientFactory(ILogger logger)
        {
            _logger = logger;
            _resolver = new KeyVaultPamValueResolver(logger);
        }

        public SecretClient Create(Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);

            _logger.LogDebug("Instantiating new instance of SecretClient...");

            string keyVaultUri = _resolver.GetValueFromDictionary(initializationInfo, "initializationInfo", "KeyVaultUri");
            _logger.LogDebug($"KeyVaultUri: {keyVaultUri}");

            SecretClient secretClient = new SecretClient(new Uri(keyVaultUri), GetTokenCredentials(initializationInfo).Key);

            _logger.LogDebug("Finished instantiating SecretClient.");

            _logger.MethodExit(LogLevel.Debug);

            return secretClient;
        }

        internal KeyValuePair<TokenCredential, Type> GetTokenCredentials(Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            _logger.LogDebug("Resolving Azure client credentials...");

            TokenCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                AuthorityHost = GetAzureAuthorityHost(initializationInfo)
            };
            TokenCredential credentials = new DefaultAzureCredential((DefaultAzureCredentialOptions) options);
            Type credentialsType = typeof(DefaultAzureCredential);
            
            if (initializationInfo.ContainsKey("TenantId") 
                    && initializationInfo.ContainsKey("ClientId")
                    && initializationInfo.ContainsKey("ClientSecret"))
            {
                _logger.LogDebug($"Found TenantId, ClientId and ClientSecret in initialization info. Setting up client secret credentials.");
                
                string tenantId = _resolver.GetValueFromDictionary(initializationInfo, "initializationInfo", "TenantId");
                string clientId = _resolver.GetValueFromDictionary(initializationInfo, "initializationInfo", "ClientId");
                string clientSecret = _resolver.GetValueFromDictionary(initializationInfo, "initializationInfo", "ClientSecret");
                
                credentials = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
                credentialsType = typeof(ClientSecretCredential);
            }
            else
            {
                _logger.LogDebug($"Using default client credentials.");
            }
            
            _logger.LogDebug("Finished resolving Azure client credentials...");
            
            _logger.MethodExit(LogLevel.Debug);

            return new KeyValuePair<TokenCredential, Type>(credentials, credentialsType);
        }

        internal Uri GetAzureAuthorityHost(Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            string authorityHost = _resolver.GetValueFromDictionaryOrEnvironment(initializationInfo, "initializationInfo", 
                "AuthorityHost", "AZURE_AUTHORITY_HOST");

            if (string.IsNullOrEmpty(authorityHost))
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
    }
}

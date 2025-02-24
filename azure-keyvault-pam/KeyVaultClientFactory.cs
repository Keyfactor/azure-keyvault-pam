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

using System;
using System.Collections.Generic;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    /// <summary>
    /// Factory class for creating and configuring Azure Key Vault SecretClient instances.
    /// </summary>
    /// <remarks>
    /// This factory handles the creation of properly authenticated SecretClient instances,
    /// supporting both service principal and default Azure credentials.
    /// </remarks>
    public class KeyVaultClientFactory
    {
        /// <summary>
        /// Logger instance for diagnostic output.
        /// </summary>
        private readonly ILogger _logger;
        
        /// <summary>
        /// Helper for resolving configuration values.
        /// </summary>
        private readonly KeyVaultPamValueResolver _resolver;
        
        /// <summary>
        /// Dictionary containing initialization parameters for the factory.
        /// </summary>
        private readonly Dictionary<string, string> _initializationInfo;

        /// <summary>
        /// Initializes a new instance of the KeyVaultClientFactory class.
        /// </summary>
        /// <param name="logger">Logger for diagnostic output</param>
        /// <param name="initializationInfo">Dictionary containing initialization parameters</param>
        public KeyVaultClientFactory(ILogger logger, Dictionary<string, string> initializationInfo)
        {
            _logger = logger;
            _resolver = new KeyVaultPamValueResolver(logger);
            _initializationInfo = initializationInfo;
        }

        /// <summary>
        /// Creates a new SecretClient instance with the configured credentials.
        /// </summary>
        /// <returns>Configured SecretClient instance for Azure Key Vault operations</returns>
        /// <remarks>
        /// The method configures a SecretClient with:
        /// 1. The KeyVaultUri from initialization parameters
        /// 2. Appropriate credentials based on available authentication information
        /// 3. Custom authority host if specified
        /// </remarks>
        /// <exception cref="KeyVaultPamException">
        /// Thrown when:
        /// - KeyVaultUri is missing or invalid
        /// - Required service principal credentials are incomplete
        /// - Custom authority host is invalid
        /// </exception>
        public SecretClient Create()
        {
            _logger.MethodEntry(LogLevel.Debug);

            _logger.LogDebug("Instantiating new instance of SecretClient...");

            string keyVaultUri = _resolver.GetValueFromDictionary(_initializationInfo, "initializationInfo", "KeyVaultUri");
            _logger.LogDebug($"KeyVaultUri: {keyVaultUri}");

            SecretClient secretClient = new SecretClient(new Uri(keyVaultUri), GetTokenCredentials().Key);

            _logger.LogDebug("Finished instantiating SecretClient.");

            _logger.MethodExit(LogLevel.Debug);

            return secretClient;
        }

        /// <summary>
        /// Gets the appropriate TokenCredential based on provided initialization parameters.
        /// </summary>
        /// <returns>A KeyValuePair containing the TokenCredential and its Type</returns>
        /// <remarks>
        /// The method attempts to create credentials in the following order:
        /// 1. Service Principal credentials if TenantId, ClientId, and ClientSecret are provided
        /// 2. DefaultAzureCredential if no service principal credentials are found
        /// </remarks>
        /// <exception cref="KeyVaultPamException">Thrown when service principal credentials are partially provided</exception>
        internal KeyValuePair<TokenCredential, Type> GetTokenCredentials()
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            _logger.LogDebug("Resolving Azure client credentials...");

            TokenCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                AuthorityHost = GetAzureAuthorityHost()
            };
            TokenCredential credentials = new DefaultAzureCredential((DefaultAzureCredentialOptions) options);
            Type credentialsType = typeof(DefaultAzureCredential);
            
            if (_initializationInfo.ContainsKey("TenantId") 
                    && _initializationInfo.ContainsKey("ClientId")
                    && _initializationInfo.ContainsKey("ClientSecret"))
            {
                _logger.LogDebug($"Found TenantId, ClientId and ClientSecret in initialization info. Setting up client secret credentials.");
                
                string tenantId = _resolver.GetValueFromDictionaryOrEnvironment(_initializationInfo, "initializationInfo", "TenantId", "AZURE_TENANT_ID");
                string clientId = _resolver.GetValueFromDictionaryOrEnvironment(_initializationInfo, "initializationInfo", "ClientId", "AZURE_CLIENT_ID");
                string clientSecret = _resolver.GetValueFromDictionaryOrEnvironment(_initializationInfo, "initializationInfo", "ClientSecret", "AZURE_CLIENT_SECRET");
                
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

        /// <summary>
        /// Resolves the Azure authority host URI based on configuration or environment variables.
        /// </summary>
        /// <returns>The resolved authority host URI or null for default</returns>
        /// <remarks>
        /// Supports the following predefined values:
        /// - "china" for Azure China
        /// - "government" for Azure Government
        /// - "public" for Azure Public Cloud
        /// Custom HTTPS URIs are also supported.
        /// </remarks>
        /// <exception cref="KeyVaultPamException">Thrown when custom authority host is invalid</exception>
        internal Uri GetAzureAuthorityHost()
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            string authorityHost = _resolver.GetValueFromDictionaryOrEnvironment(_initializationInfo, "initializationInfo", 
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

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
using System.Diagnostics.CodeAnalysis;
using Azure.Core;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    /// <summary>
    /// Interface for Password Authority Manager (PAM) providers that can retrieve secrets from various vault systems.
    /// </summary>
    public interface IPAMProvider
    {
        /// <summary>
        /// Asynchronously retrieves a password/secret from the PAM provider.
        /// </summary>
        /// <param name="instanceParameters">Parameters specific to the secret being retrieved</param>
        /// <param name="initializationInfo">Parameters required to initialize the PAM provider</param>
        /// <returns>The retrieved secret value</returns>
        Task<string> GetPasswordAsync(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo);
    }

    /// <summary>
    /// Base class for Azure Key Vault PAM providers that implements common functionality.
    /// </summary>
    /// <remarks>
    /// This class handles the core functionality of connecting to and retrieving secrets from Azure Key Vault.
    /// It supports both synchronous and asynchronous operations, and properly manages the lifecycle of the SecretClient.
    /// </remarks>
    public abstract class KeyVaultPamCommon : IPAMProvider, IDisposable
    {
        /// <summary>
        /// Logger instance for diagnostic output.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Client for accessing Azure Key Vault secrets.
        /// </summary>
        private SecretClient _secretClient;

        /// <summary>
        /// Helper for resolving values from dictionaries and environment variables.
        /// </summary>
        private readonly KeyVaultPamValueResolver _resolver;

        /// <summary>
        /// Flag to track whether the instance has been disposed.
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// Gets the name of the PAM provider implementation.
        /// </summary>
        public abstract string Name { get; }

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
        public async Task<string> GetPasswordAsync(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry(LogLevel.Debug);

            if (instanceParameters == null)
                throw new ArgumentNullException(nameof(instanceParameters));
            if (initializationInfo == null)
                throw new ArgumentNullException(nameof(initializationInfo));

            string password = await GetAzureKeyVaultSecret(instanceParameters, initializationInfo);

            _logger.MethodExit(LogLevel.Debug);

            return password;
        }

        /// <summary>
        /// Synchronously retrieves a password value from Azure Key Vault.
        /// </summary>
        /// <remarks>
        /// This is a blocking wrapper around GetPasswordAsync for backward compatibility.
        /// For better performance, prefer using GetPasswordAsync when possible.
        /// </remarks>
        /// <param name="instanceParameters">Dictionary containing instance-specific parameters</param>
        /// <param name="initializationInfo">Dictionary containing initialization parameters</param>
        /// <returns>The retrieved secret value</returns>
        /// <exception cref="ArgumentNullException">Thrown when either parameter dictionary is null</exception>
        /// <exception cref="KeyVaultPamException">Thrown when required values are missing</exception>
        public string GetPassword(Dictionary<string, string> instanceParameters,
            Dictionary<string, string> initializationInfo)
        {
            return GetPasswordAsync(instanceParameters, initializationInfo)
                .GetAwaiter()
                .GetResult();
        }

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
            _logger.MethodEntry(LogLevel.Debug);
            
            _logger.LogDebug($"Using PAM Provider: {Name}");

            if (_secretClient is null)
            {
                _secretClient = new KeyVaultClientFactory(_logger, initializationInfo).Create();
            }

            string secretId = _resolver.GetValueFromDictionary(instanceParameters, "instanceParameters", "SecretId");
            _logger.LogDebug($"SecretId: {secretId}");

            _logger.LogDebug("Getting secret from Azure Key Vault...");

            string keyVaultUri = _resolver.GetValueFromDictionary(initializationInfo, "initializationInfo", "KeyVaultUri");
            _logger.LogDebug($"KeyVaultUri: {keyVaultUri}");

            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new KeyVaultPamException("KeyVaultUri is required but was null or empty");
            }

            if (!Uri.TryCreate(keyVaultUri, UriKind.Absolute, out Uri uri) || 
                !uri.Host.EndsWith(".vault.azure.net"))
            {
                throw new KeyVaultPamException($"Invalid Key Vault URI format: {keyVaultUri}");
            }

            KeyVaultSecret secret = await _secretClient
                .GetSecretAsync(secretId)
                .ConfigureAwait(false);

            _logger.LogDebug("Finished getting secret from Azure Key Vault.");

            _logger.MethodExit(LogLevel.Debug);

            return secret.Value;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the KeyVaultPamCommon and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _secretClient?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

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
        /// Creates a new instance of KeyVaultClientFactory.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostic output</param>
        /// <param name="initializationInfo">Dictionary containing initialization parameters including KeyVaultUri and optional authentication details</param>
        public KeyVaultClientFactory(ILogger logger, Dictionary<string, string> initializationInfo)
        {
            // Implementation of the constructor
        }

        /// <summary>
        /// Creates a new SecretClient instance configured with the appropriate credentials and Key Vault URI.
        /// </summary>
        /// <returns>Configured SecretClient instance</returns>
        /// <exception cref="KeyVaultPamException">Thrown when KeyVaultUri is invalid or missing</exception>
        public SecretClient Create()
        {
            // Implementation of the Create method
            return null; // Placeholder return, actual implementation needed
        }

        /// <summary>
        /// Gets the appropriate TokenCredential based on provided initialization parameters.
        /// Falls back to DefaultAzureCredential if no service principal credentials are provided.
        /// </summary>
        /// <returns>A KeyValuePair containing the TokenCredential and its Type</returns>
        internal KeyValuePair<TokenCredential, Type> GetTokenCredentials()
        {
            // Implementation of the GetTokenCredentials method
            return default; // Placeholder return, actual implementation needed
        }

        /// <summary>
        /// Resolves the Azure authority host URI based on configuration or environment variables.
        /// Supports predefined values (china, government, public) or custom HTTPS URIs.
        /// </summary>
        /// <returns>The resolved authority host URI or null for default</returns>
        /// <exception cref="KeyVaultPamException">Thrown when custom authority host is invalid</exception>
        internal Uri GetAzureAuthorityHost()
        {
            // Implementation of the GetAzureAuthorityHost method
            return null; // Placeholder return, actual implementation needed
        }
    }

    /// <summary>
    /// Azure Key Vault PAM provider that uses DefaultAzureCredential for authentication.
    /// </summary>
    /// <remarks>
    /// This is the recommended provider as it supports multiple authentication methods including
    /// managed identities, environment credentials, and Visual Studio/CLI credentials.
    /// </remarks>
    public class KeyVaultPam : KeyVaultPamCommon
    {
        // Implementation of the KeyVaultPam class
    }

    /// <summary>
    /// Azure Key Vault PAM provider that uses service principal (client credentials) authentication.
    /// </summary>
    /// <remarks>
    /// This provider requires explicit service principal credentials (TenantId, ClientId, ClientSecret)
    /// to be provided either in initialization parameters or environment variables.
    /// </remarks>
    public class KeyVaultPamServicePrincipal : KeyVaultPamCommon
    {
        // Implementation of the KeyVaultPamServicePrincipal class
    }
}

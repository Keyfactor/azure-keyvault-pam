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

using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    /// <summary>
    /// Default Azure Key Vault PAM provider using DefaultAzureCredential authentication.
    /// </summary>
    /// <remarks>
    /// This is the recommended provider as it supports multiple authentication methods:
    /// - Managed Identity
    /// - Environment Variables
    /// - Visual Studio Credentials
    /// - Azure CLI Credentials
    /// - Interactive Browser Login
    /// </remarks>
    public class KeyVaultPam : KeyVaultPamCommon
    {
        /// <summary>
        /// Logger instance for diagnostic output and error tracking.
        /// </summary>
        private static readonly ILogger Logger = LogHandler.GetClassLogger<KeyVaultPam>();

        /// <summary>
        /// Initializes a new instance of the KeyVaultPam class using default authentication.
        /// </summary>
        public KeyVaultPam(): base(Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KeyVaultPam class with a pre-configured SecretClient.
        /// </summary>
        /// <param name="client">Pre-configured Azure Key Vault SecretClient</param>
        public KeyVaultPam(SecretClient client): base(Logger, client)
        {
        }

        /// <summary>
        /// Gets the name identifier for this PAM provider implementation.
        /// </summary>
        /// <value>The string "Azure-KeyVault"</value>
        public override string Name => "Azure-KeyVault";

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
    }
}

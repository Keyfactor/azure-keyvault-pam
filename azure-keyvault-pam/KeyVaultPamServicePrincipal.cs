// Copyright 2025 Keyfactor
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

using Azure.Security.KeyVault.Secrets;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    /// <summary>
    /// Azure Key Vault PAM provider using service principal authentication.
    /// </summary>
    /// <remarks>
    /// This provider requires explicit service principal credentials:
    /// - TenantId
    /// - ClientId
    /// - ClientSecret
    /// These can be provided via initialization parameters or environment variables.
    /// </remarks>
    public class KeyVaultPamServicePrincipal : KeyVaultPamCommon
    {
        private static readonly ILogger Logger = LogHandler.GetClassLogger<KeyVaultPamServicePrincipal>();

        /// <summary>
        /// Initializes a new instance of the KeyVaultPamServicePrincipal class.
        /// </summary>
        /// <remarks>
        /// When using this constructor, service principal credentials must be provided
        /// through initialization parameters or environment variables.
        /// </remarks>
        public KeyVaultPamServicePrincipal(): base(Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the KeyVaultPamServicePrincipal class with a pre-configured SecretClient.
        /// </summary>
        /// <param name="client">Pre-configured Azure Key Vault SecretClient</param>
        /// <remarks>
        /// Use this constructor when you need to provide a custom-configured SecretClient,
        /// such as when using different authentication options or client configurations.
        /// </remarks>
        public KeyVaultPamServicePrincipal(SecretClient client): base(Logger, client)
        {
        }

        public override string Name => "Azure-KeyVault-ServicePrincipal";

        
    }
}

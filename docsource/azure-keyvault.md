## Overview

The default Azure Key Vault PAM extension is the recommended extension. This extension is the most flexible. It is able to assume credentials from your machine's Azure role or via environment variables if configured.

## Requirements

The Azure Key Vault PAM extension requires a Key Vault hosted in Azure (Public / Government / China) or a Key Vault hosted with Azure Key Vault-compatible APIs. To access your Key Vault, permissions will need to be configured to allow your machine to the Key Vault (details found below).

An Azure Key Vault can be easily created and configured within Azure (documentation for how to create a key vault in the Azure Portal can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/general/quick-create-portal)). Each Azure Key Vault will have its own unique endpoint (Vault URI) which is visible from the key vault's _Overview_ section. 

New secrets can be added to your Azure Key Vault under the key vault's _Secrets_ section. Documentation on how to create a secret in Azure Portal can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-portal#add-a-secret-to-key-vault).

You can either use Role-Based Access Control (RBAC) or Access Policies to manage access to your Key Vault secrets. Documentation on access policies for secrets can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/secrets/about-secrets#secret-access-control) while documentation on RBAC access to secrets can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide).

If your app is hosted in Azure, follow [this guide](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/azure-hosted-apps) on how to authenticate your application with your Azure resources.

If your app is ***not hosted*** in Azure, you can follow [this guide](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/on-premises-apps) on how to authenticate your non-Azure / on-premise application with your Azure resources.

### Initialization and Instance Parameters for Extension

__Initialization Parameters for each defined PAM Provider instance__
| Initialization parameter | Display Name | Description |
| :---: | :---: | --- |
| KeyVaultUri | Azure Key Vault URI | The unique auto generated URI for your Azure KeyVault. |
| AuthorityHost | Authority Host | The authority host to authenticate against. For most use cases, this will simply be `public`. Please refer to the **Authority Hosts** section for more information on this parameter. If `AZURE_AUTHORITY_HOST` is a defined environment variable, it will override this value. |

__Instance Parameters for each retrieved secret field__
| Instance parameter | Display Name | Description |
| :---: | :---: | --- |
| SecretId | Secret Name | The name of the secret you assigned in Azure Key Vault. |

## Platform Install

The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension. Modify your `manifest.json`, updating the `InitializationInfo` section with the appropriate values.

## Orchestrator Install

The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension.


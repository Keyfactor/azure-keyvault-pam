## Overview

The Azure Key Vault PAM Provider uses the Azure Key Vault SDK to communicate with a Key Vault in Azure. The provider is able to communicate with Azure Public Cloud, Government, and China. Alternatively, if you have a self-hosted Key Vault compatible with Key Vault's APIs, the provider should be able to communicate with the Key Vault.
Communication with Azure Key Vault is supported via assuming a role on your machine, by reading credentials in environment variables, or by using service principal credentials in your PAM extension configuration.

This PAM Provider supports retrieving all fields available in Azure Key Vault, such as usernames and passwords. It can be installed on either the Keyfactor Command Platform or on Universal Orchestrators.

## Azure KeyVault vs Azure KeyVault ServicePrincipal

There are two Azure Key Vault PAM Providers available: `Azure-KeyVault` and `Azure-KeyVault-ServicePrincipal`.

Here's a matrix explaining the differences between the two extensions:

| PAM Type | Recommended Use Case | manifest.json Configuration | 
|--|--|--|
| Azure-KeyVault | Recommended if Orchestrator or machine can assume an Azure role with a managed identity or read credentials from environment variables to authenticate with Azure Key Vault. [How to setup managed identity access to KeyVault](https://learn.microsoft.com/en-us/azure/key-vault/general/authentication) | - |
| Azure-KeyVault-ServicePrincipal | Recommended if you want to directly specify service principal credentials in your PAM provider configuration to authenticate with Azure Key Vault. Useful if Orchestrator or machine cannot assume an Azure role with managed identity or have ability to modify environment variables. [How to create an Azure service principal](https://learn.microsoft.com/en-us/entra/identity-platform/app-objects-and-service-principals?tabs=browser) | Replace `manifest.json` with contents of `ServicePrincipal-manifest.json` |

## Environment Variable Configuration

Both PAM providers support authenticating with Azure Key Vault via environment variables. If the appropriate environment variables are configured, the PAM provider will read credentials from the environment variables to authenticate with Azure Key Vault. Environment variables will take precedence over the initialization parameters (i.e. `Azure-KeyVault-ServicePrincipal`). The supported environment variables for both extensions are:

| Environment Variable | Description |
|--|--|
| AZURE_CLIENT_ID | The application (client) ID of an Azure AD application. |
| AZURE_TENANT_ID | The tenant (directory) ID in Azure the Azure Key Vault belongs to. |
| AZURE_CLIENT_SECRET | The client secret for the Azure AD application. |
| AZURE_AUTHORITY_HOST | The authority host to authenticate against. For most use cases, this will simply be `public`. Please refer to the [Authority Hosts](#authority-hosts) section for more information on this parameter. |

## Authority Hosts

The Azure Key Vault PAM provider requires an **Authority Host** to be defined. The **Authority Host** is the endpoint with which Azure will authenticate against. There are predefined Azure Authority Hosts the PAM Provider library will resolve to. The value and resolved Authority Host can be found below:

|Value|Authority Host|
|--|--|
|china|Azure China|
|government|Azure Government|
|public|Azure Public Cloud|

For most use cases, `public` will be an acceptable **Authority Host** value for your PAM provider. You may also provide a custom authority host not defined in the table above, but the authority host ***must*** begin with `https://`, for example `https://custom.microsoftonline.com`.

Authority Hosts may also be specified via the `AZURE_AUTHORITY_HOST` environment variable. If this environment variable is configured, it will override the value supplied to the PAM provider.

For more information on Azure authority hosts, please review [the Azure SDK documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet#properties).

## Example Setup of Azure Key Vault PAM Provider

This example shows setting up a service principal access to an Azure Key Vault. This example only covers using [RBAC / Access Control (IAM)](https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide?tabs=azure-cli), but you can also use [Access Policies](https://learn.microsoft.com/en-us/azure/key-vault/general/assign-access-policy?tabs=azure-portal) to configure access to your Key Vault.

First, within Entra ID, create a service principal by creating an app registration. Once the app registration is created, create a client secret for the app registration and note the client secret value, application (client) ID, and tenant ID.

![Register App](images/setup/register.png)
![Client ID and Tenant ID](images/setup/clientid.png)

Navigate to **Certificates & Secrets** and create a new client secret. Note the value of the client secret as it will not be shown again after you navigate away from the page. **Ignore the Secret ID shown on this page as it is not used for PAM provider configuration.**

![Client Secret](images/setup/clientsecret.png)

Now, navigate to your Azure Key Vault instance. Under the **Access Control (IAM)** section, add a new role assignment. You can assign the "Key Vault Secrets User" role, which will allow the service principal to read secrets from the Key Vault. Assign this role to the service principal you created in the previous step.


![Role Assignment](images/setup/roleassignment.png)

Note the Key Vault URI from the Key Vault's overview page as you will need it for PAM provider configuration.

![Key Vault URI](images/setup/keyvault.png)

Next, add a secret to your Key Vault or use an existing secret. Note the name of the secret as you will need it for PAM provider configuration (Secret ID).

![Secret](images/setup/secret.png)

Finally, configure your PAM provider with the appropriate initialization and instance parameters as outlined in the [Initialization and Instance Parameters for Extension](#initialization-and-instance-parameters-for-extension) section. If you are using the `Azure-KeyVault-ServicePrincipal` PAM provider, you can directly input the service principal credentials in the initialization parameters. If you are using the `Azure-KeyVault` PAM provider, you can set the service principal credentials as environment variables on your machine or Orchestrator.

![PAM Provider](images/setup/pam-provider.png)
![PAM Usage](images/setup/pam-usage.png)
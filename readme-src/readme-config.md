---
### readme-src/readme-config.md
---
### Configuring for PAM Usage
#### In Azure Key Vault
An Azure Key Vault can be easily created and configured within Azure (documentation for how to create a key vault in the Azure Portal can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/general/quick-create-portal)). Each Azure Key Vault will have its own unique endpoint (Vault URI) which is visible from the key vault's _Overview_ section. 

New secrets can be added to your Azure Key Vault under the key vault's _Secrets_ section. Documentation on how to create a secret in Azure Portal can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-portal#add-a-secret-to-key-vault).

#### Authentication

You can either use Role-Based Access Control (RBAC) or Access Policies to manage access to your Key Vault secrets. Documentation on access policies for secrets can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/secrets/about-secrets#secret-access-control) while documentation on RBAC access to secrets can be found [here](https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide).

If your app is hosted in Azure, follow [this guide](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/azure-hosted-apps) on how to authenticate your application with your Azure resources.

If your app is ***not hosted*** in Azure, you can follow [this guide](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/on-premises-apps) on how to authenticate your non-Azure / on-premise application with your Azure resources.

##### Standard vs Service Principal PAM Provider

This PAM provider comes in two forms: a **Service Principal** Azure Key Vault PAM Provider and a **Standard** Azure Key Vault PAM Provider.

If you are using the **Standard** Azure Key Vault PAM Provider, you are all set and you can skip this section.

The **Service Principal** Azure Key Vault PAM Provider has the provider name `Azure-KeyVault-ServicePrincipal`. This type of PAM Provider allows the end user to configure the PAM provider with a `ClientId`, `ClientSecret`, and `TenantId`, which can be entered in Command or provided in the `manifest.json`. The manifest definition for this type of provider can be found in the `ServicePrincipal-manifest.json`. If you are using this type of PAM provider, merge the definition into the `manifest.json`.

**NOTE**: If your system has `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, and `AZURE_TENANT_ID` environment values defined, these environment variables will override the initialization information for `ClientId`, `ClientSecret`, and `TenantId`, respectively.

##### Authority Hosts

The Azure Key Vault PAM provider requires an **Authority Host** to be defined. The **Authority Host** is the endpoint with which Azure will authenticate against. There are predefined Azure Authority Hosts the PAM Provider library will resolve to. The value and resolved Authority Host can be found below:

|Value|Authority Host|
|--|--|
|china|Azure China|
|government|Azure Government|
|public|Azure Public Cloud|

For most use cases, `public` will be an acceptable **Authority Host** value for your PAM provider. You may also provide a custom authority host not defined in the table above, but the authority host ***must*** begin with `https://`, for example `https://custom.microsoftonline.com`.

Authority Hosts may also be specified via the `AZURE_AUTHORITY_HOST` environment variable. If this environment variable is configured, it will override the value supplied to the PAM provider.

For more information on Azure authority hosts, please review [the Azure SDK documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet#properties).


#### Usage with the Keyfactor Universal Orchestrator

To use the PAM Provider to resolve a field, for example a Server Username or Server Password, instead of entering in the actual value for the Server Password, enter a `json` object with the parameters specifying the field. For Azure Key Vault, you will use the **name** of your secret.

The parameters needed are the "instance" parameters above:

~~~ json
{"SecretId":"name-of-azure-key-vault-secret"}
~~~

If a field supports PAM but should not use PAM, simply enter in the actual value to be used instead of the `json` format object above.

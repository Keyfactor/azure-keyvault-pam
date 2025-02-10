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


##### Authority Hosts

The Azure Key Vault PAM provider allows you to specify an authority host for the Azure SDK to authenticate with. To configure the authority host you want the PAM provider to interact with, set the `AZURE_AUTHORITY_HOST` environment variable to a value from the below list or to a custom value of your choosing. If you wish to use a custom authority host, the scheme of the host ***must*** begin with `https://`.

|Value|Authority Host|
|--|--|
|china|Azure China|
|government|Azure Government|
|public|Azure Public Cloud|

For more information on Azure authority hosts, please review [the Azure SDK documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet#properties).


#### Usage with the Keyfactor Universal Orchestrator
To use the PAM Provider to resolve a field, for example a Server Password, instead of entering in the actual value for the Server Password, enter a `json` object with the parameters specifying the field. For Azure Key Vault, you will use the **name** of your secret.
The parameters needed are the "instance" parameters above:

~~~ json
{"SecretId":"name-of-azure-key-vault-secret"}
~~~

If a field supports PAM but should not use PAM, simply enter in the actual value to be used instead of the `json` format object above.
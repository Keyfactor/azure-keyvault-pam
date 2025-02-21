<h1 align="center" style="border-bottom: none">
    Azure Key Vault PAM Provider
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-prototype-3D1973?style=flat-square" alt="Integration Status: prototype" />
<a href="https://github.com/Keyfactor/azure-keyvault-pam/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/azure-keyvault-pam?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/azure-keyvault-pam?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/azure-keyvault-pam/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a> 
  ·
  <a href="#getting-started">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=pam">
    <b>Related Integrations</b>
  </a>
</p>

## Overview

The Azure Key Vault PAM Provider uses the Azure Key Vault SDK to communicate with a Key Vault in Azure. The provider is able to communicate with Azure Public Cloud, Government, and China. Alternatively, if you have a self-hosted Key Vault compatible with Key Vault's APIs, the provider should be able to communicate with the Key Vault.
Communication with Azure Key Vault is supported via assuming a role on your machine, by reading credentials in environment variables, or by using service principal credentials in your PAM extension configuration.

This PAM Provider supports retrieving all fields available in Azure Key Vault, such as usernames and passwords. It can be installed on either the Keyfactor Command Platform or on Universal Orchestrators.

## Support
The Azure Key Vault PAM Provider is open source and there is **no SLA**. Keyfactor will address issues as resources become available. Keyfactor customers may request escalation by opening up a support ticket through their Keyfactor representative. 

> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Getting Started

The Azure Key Vault PAM Provider is used by Command to resolve PAM-eligible credentials for Universal Orchestrator extensions and for accessing Certificate Authorities. When configured, Command will use the Azure Key Vault PAM Provider to retrieve credentials needed to communicate with the target system. There are two ways to install the Azure Key Vault PAM Provider, and you may elect to use one or both methods:

1. **Locally on the Keyfactor Command server**: PAM credential resolution via the Azure Key Vault PAM Provider will occur on the Keyfactor Command server each time an elegible credential is needed.
2. **Remotely On Universal Orchestrators**: When Jobs are dispatched to Universal Orchestrators, the associated Certificate Store extension assembly will use the Azure Key Vault PAM Provider to resolve eligible PAM credentials.

Before proceeding with installation, you should consider which pattern is best for your requirements and use case.

### Installation

To install Azure Key Vault PAM Provider, you must install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating PAM Types in Keyfactor Command, among many other useful automation features.

The Azure Key Vault PAM Provider implements 2 PAM Types. Depending on your use case, you may elect to install one, or all of these PAM Types. An overview for each type is linked below:
* [Azure-KeyVault](docs/azure-keyvault.md)
* [Azure-KeyVault-ServicePrincipal](docs/azure-keyvault-serviceprincipal.md)






<details><summary>Azure-KeyVault</summary>


#### Prerequisites

1. Follow the [requirements section](docs/azure-keyvault.md#requirements) to configure a Service Account, grant necessary API permissions, and create secrets.

    <details><summary>Requirements</summary>
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

    ### Authority Hosts

    The Azure Key Vault PAM provider requires an **Authority Host** to be defined. The **Authority Host** is the endpoint with which Azure will authenticate against. There are predefined Azure Authority Hosts the PAM Provider library will resolve to. The value and resolved Authority Host can be found below:

    |Value|Authority Host|
    |--|--|
    |china|Azure China|
    |government|Azure Government|
    |public|Azure Public Cloud|

    For most use cases, `public` will be an acceptable **Authority Host** value for your PAM provider. You may also provide a custom authority host not defined in the table above, but the authority host ***must*** begin with `https://`, for example `https://custom.microsoftonline.com`.

    Authority Hosts may also be specified via the `AZURE_AUTHORITY_HOST` environment variable. If this environment variable is configured, it will override the value supplied to the PAM provider.

    For more information on Azure authority hosts, please review [the Azure SDK documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet#properties).

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # Azure-KeyVault
    kfutil pam types-create -r azure-keyvault-pam -n Azure-KeyVault
    ```

#### Install on Keyfactor Command (Local)


('The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension. Modify your `manifest.json`, updating the `InitializationInfo` section with the appropriate values.',)


#### Install on a Universal Orchestrator (Remote)


('The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension.',)



</details>







<details><summary>Azure-KeyVault-ServicePrincipal</summary>


#### Prerequisites

1. Follow the [requirements section](docs/azure-keyvault-serviceprincipal.md#requirements) to configure a Service Account, grant necessary API permissions, and create secrets.

    <details><summary>Requirements</summary>
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
    | AuthorityHost | Authority Host | The authority host to authenticate against. For most use cases, this will simply be `public`. Please refer to the **Authority Host** section for more information on this parameter. If `AZURE_AUTHORITY_HOST` is a defined environment variable, it will override this value. |
    | TenantId | Tenant ID | The tenant (directory) ID in Azure the Azure Key Vault belongs to. If `AZURE_TENANT_ID` is a defined environment variable, it will override this value. |
    | ClientId | Client ID | The application ID in Entra AD. If `AZURE_CLIENT_ID` is a defined environment variable, it will override this value. |
    | ClientSecret | Client Secret | The client secret for the application ID. If `AZURE_CLIENT_SECRET` is a defined environment variable, it will override this value. |

    __Instance Parameters for each retrieved secret field__
    | Instance parameter | Display Name | Description |
    | :---: | :---: | --- |
    | SecretId | Secret Name | The name of the secret you assigned in Azure Key Vault. |

    ### Authority Hosts

    The Azure Key Vault PAM provider requires an **Authority Host** to be defined. The **Authority Host** is the endpoint with which Azure will authenticate against. There are predefined Azure Authority Hosts the PAM Provider library will resolve to. The value and resolved Authority Host can be found below:

    |Value|Authority Host|
    |--|--|
    |china|Azure China|
    |government|Azure Government|
    |public|Azure Public Cloud|

    For most use cases, `public` will be an acceptable **Authority Host** value for your PAM provider. You may also provide a custom authority host not defined in the table above, but the authority host ***must*** begin with `https://`, for example `https://custom.microsoftonline.com`.

    Authority Hosts may also be specified via the `AZURE_AUTHORITY_HOST` environment variable. If this environment variable is configured, it will override the value supplied to the PAM provider.

    For more information on Azure authority hosts, please review [the Azure SDK documentation](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet#properties).

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # Azure-KeyVault-ServicePrincipal
    kfutil pam types-create -r azure-keyvault-pam -n Azure-KeyVault-ServicePrincipal
    ```

#### Install on Keyfactor Command (Local)


('The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension. Copy the `ServicePrincipal-manifest.json` into your `manifest.json` file, and then update the `InitializationInfo` section with the appropriate values.',)


#### Install on a Universal Orchestrator (Remote)


('The entire contents (which includes all library dependencies) should be copied when installing. Refer to the [Keyfactor Command documentation](https://software.keyfactor.com/Core-OnPrem/v24.4.1/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm) on how to install your extension. Copy the `ServicePrincipal-manifest.json` into your `manifest.json` file.',)



</details>





### Usage





<details><summary>Azure-KeyVault</summary>


#### Keyfactor Command (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **Azure-KeyVault**.

    > If you're running Keyfactor Command 11+, make sure "Remote Provider" is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/azure-keyvault.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| KeyVaultUri | Key Vault URI | URI for your Azure Key Vault |
| AuthorityHost | Authority Host | Authority host of your Azure infrastructure |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **Azure-KeyVault** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| SecretId | Secret ID | Name of your secret in Azure Key Vault |





#### Universal Orchestrator (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the Azure-KeyVault PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that "Remote Provider" is checked.

4. Click the dropdown for **Provider Type** and select **Azure-KeyVault**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **Azure-KeyVault** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| SecretId | Secret ID | Name of your secret in Azure Key Vault |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"SecretId": "Name of your secret in Azure Key Vault"}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> Additional information on Azure-KeyVault can be found in the [supplimental documentation](docs/azure-keyvault.md).





<details><summary>Azure-KeyVault-ServicePrincipal</summary>


#### Keyfactor Command (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **Azure-KeyVault-ServicePrincipal**.

    > If you're running Keyfactor Command 11+, make sure "Remote Provider" is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/azure-keyvault-serviceprincipal.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| KeyVaultUri | Key Vault URI | URI for your Azure Key Vault |
| AuthorityHost | Authority Host | Authority host of your Azure infrastructure |
| TenantId | Tenant ID | Tenant or directory ID in Azure |
| ClientId | Client ID | Application ID in Entra AD |
| ClientSecret | ClientSecret | Client secret for your application ID |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault-ServicePrincipal** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **Azure-KeyVault-ServicePrincipal** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| SecretId | Secret ID | Name of your secret in Azure Key Vault |





#### Universal Orchestrator (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the Azure-KeyVault-ServicePrincipal PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that "Remote Provider" is checked.

4. Click the dropdown for **Provider Type** and select **Azure-KeyVault-ServicePrincipal**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault-ServicePrincipal** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **Azure-KeyVault-ServicePrincipal** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| SecretId | Secret ID | Name of your secret in Azure Key Vault |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **Azure-KeyVault-ServicePrincipal** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"SecretId": "Name of your secret in Azure Key Vault"}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> Additional information on Azure-KeyVault-ServicePrincipal can be found in the [supplimental documentation](docs/azure-keyvault-serviceprincipal.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).
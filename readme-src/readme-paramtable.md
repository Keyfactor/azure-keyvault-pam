---
### readme-src/readme-paramtable.md
---
#### Standard Azure Key Vault Provider
__Initialization Parameters for each defined PAM Provider instance__
| Initialization parameter | Display Name | Description |
| :---: | :---: | --- |
| KeyVaultUri | Azure Key Vault URI | The unique auto generated URI for your Azure KeyVault. |
| AuthorityHost | Authority Host | The authority host to authenticate against. For most use cases, this will simply be `public`. Please refer to the **Authority Host** section for more information on this parameter. If `AZURE_AUTHORITY_HOST` is a defined environment variable, it will override this value. |

__Instance Parameters for each retrieved secret field__
| Instance parameter | Display Name | Description |
| :---: | :---: | --- |
| SecretId | Secret Name | The name of the secret you assigned in Azure Key Vault. |

When using the Service Principal Azure Key Vault Provider, the parameters are instead defined as follows:
#### Service Principal Azure Key Vault Provider
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

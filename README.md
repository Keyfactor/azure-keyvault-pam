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

TODO this section is required

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
    TODO Requirements is a required section

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # Azure-KeyVault
    kfutil pam types-create -r azure-keyvault-pam -n Azure-KeyVault
    ```

#### Install on Keyfactor Command (Local)


("TODO Platform Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Install on a Universal Orchestrator (Remote)


("TODO Orchestrator Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>







<details><summary>Azure-KeyVault-ServicePrincipal</summary>


#### Prerequisites

1. Follow the [requirements section](docs/azure-keyvault-serviceprincipal.md#requirements) to configure a Service Account, grant necessary API permissions, and create secrets.

    <details><summary>Requirements</summary>
    TODO Requirements is a required section

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # Azure-KeyVault-ServicePrincipal
    kfutil pam types-create -r azure-keyvault-pam -n Azure-KeyVault-ServicePrincipal
    ```

#### Install on Keyfactor Command (Local)


("TODO Platform Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Install on a Universal Orchestrator (Remote)


("TODO Orchestrator Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>





### Usage





<details><summary>Azure-KeyVault</summary>


#### Keyfactor Command (Local)


("TODO Platform Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Universal Orchestrator (Remote)


("TODO Orchestrator Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>


> Additional information on Azure-KeyVault can be found in the [supplimental documentation](docs/azure-keyvault.md).





<details><summary>Azure-KeyVault-ServicePrincipal</summary>


#### Keyfactor Command (Local)


("TODO Platform Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Universal Orchestrator (Remote)


("TODO Orchestrator Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>


> Additional information on Azure-KeyVault-ServicePrincipal can be found in the [supplimental documentation](docs/azure-keyvault-serviceprincipal.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).
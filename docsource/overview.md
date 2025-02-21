## Overview

The Azure Key Vault PAM Provider uses the Azure Key Vault SDK to communicate with a Key Vault in Azure. The provider is able to communicate with Azure Public Cloud, Government, and China. Alternatively, if you have a self-hosted Key Vault compatible with Key Vault's APIs, the provider should be able to communicate with the Key Vault.
Communication with Azure Key Vault is supported via assuming a role on your machine, by reading credentials in environment variables, or by using service principal credentials in your PAM extension configuration.

This PAM Provider supports retrieving all fields available in Azure Key Vault, such as usernames and passwords. It can be installed on either the Keyfactor Command Platform or on Universal Orchestrators.

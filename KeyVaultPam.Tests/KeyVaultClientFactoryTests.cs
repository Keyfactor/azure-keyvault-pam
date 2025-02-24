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

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Extensions.Pam.AzureKeyVault;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeyVaultPamTests;

public class KeyVaultClientFactoryTests
{
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly KeyVaultClientFactory _factory;
    private readonly Dictionary<string, string> _initializationInfo = new ();
    private const string MockKeyVaultUri = "https://example.azure.net/";
    private const string MockAuthorityHost = "https://example.microsoftonline.com";
    
    public KeyVaultClientFactoryTests()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", MockAuthorityHost);
        
        _factory = new KeyVaultClientFactory(_logger, _initializationInfo);
    }
    
    #region Create

    [Fact]
    public void Create_WhenCalled_ReturnsClientId()
    {
        _initializationInfo.Add("KeyVaultUri", MockKeyVaultUri);

        SecretClient client = _factory.Create();
        
        Assert.Equal(new Uri(MockKeyVaultUri), client.VaultUri);
    }
    
    #endregion Create
    
    #region GetAzureAuthorityHost

    [Fact]
    public void GetAzureAuthorityHost_WhenNoEnvironmentVariableIsPresent_AndNoInitializationInfoIsPresent_ThrowsException()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", null);
        
        Assert.Throws<KeyVaultPamException>(() => _factory.GetAzureAuthorityHost());
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsPublic_ReturnsPublicCloudHost()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "public");
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsGovernment_ReturnsGovernmentCloudHost()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "government");
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(AzureAuthorityHosts.AzureGovernment, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsChina_ReturnsChinaCloudHost()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "china");
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(AzureAuthorityHosts.AzureChina, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsCustomUri_ReturnsCustomUri()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", MockAuthorityHost);
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(new Uri(MockAuthorityHost), result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenNoEnvironmentVariableIsPresent_AndInitializationInfoIsSet_UsesInitializationInfoValue()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", null);
        
        _initializationInfo.Add("AuthorityHost", "public");
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsSet_AndInitializatioInfoIsSet_ReturnsValueFromEnvironmentVariable()
    {
        _initializationInfo.Add("AuthorityHost", "government");
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "public");
        
        Uri result = _factory.GetAzureAuthorityHost();
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    #endregion GetAzureAuthorityHost
    
    #region GetTokenCredentials

    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoIsEmpty_ReturnsDefaultCredentials()
    {
        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials();
        
        Assert.Equal(typeof(DefaultAzureCredential), result.Value);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoIsMissingAClientSecretField_ReturnsDefaultCredentials()
    {
        _initializationInfo.Add("ClientId", "foo");
        _initializationInfo.Add("ClientSecret", "bar");

        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials();
        
        Assert.Equal(typeof(DefaultAzureCredential), result.Value);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoHasAllClientSecretFields_ReturnsClientSecretCredentials()
    {
        _initializationInfo.Add("ClientId", "foo");
        _initializationInfo.Add("ClientSecret", "bar");
        _initializationInfo.Add("TenantId", "baz");

        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials();
        
        Assert.Equal(typeof(ClientSecretCredential), result.Value);
    }
    
    #endregion
}

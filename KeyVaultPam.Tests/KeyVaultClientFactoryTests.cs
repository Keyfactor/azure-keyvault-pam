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
    private const string MockKeyVaultUri = "https://example.azure.net/";
    private const string MockAuthorityHost = "https://example.microsoftonline.com";
    
    public KeyVaultClientFactoryTests()
    {
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", null);
        
        _factory = new KeyVaultClientFactory(_logger);
    }
    
    #region Create

    [Fact]
    public void Create_WhenCalled_ReturnsClientId()
    {
        Dictionary<string, string> initializationInfo = new();
        
        initializationInfo.Add("KeyVaultUri", MockKeyVaultUri);

        SecretClient client = _factory.Create(initializationInfo);
        
        Assert.Equal(new Uri(MockKeyVaultUri), client.VaultUri);
    }
    
    #endregion Create
    
    #region GetAzureAuthorityHost

    [Fact]
    public void GetAzureAuthorityHost_WhenNoEnvironmentVariableIsPresent_AndNoInitializationInfoIsPresent_ReturnsNull()
    {
        Dictionary<string, string> initializationInfo = new();
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Null(result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsPublic_ReturnsPublicCloudHost()
    {
        Dictionary<string, string> initializationInfo = new();
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "public");
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsGovernment_ReturnsGovernmentCloudHost()
    {
        Dictionary<string, string> initializationInfo = new();
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "government");
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(AzureAuthorityHosts.AzureGovernment, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsChina_ReturnsChinaCloudHost()
    {
        Dictionary<string, string> initializationInfo = new();
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "china");
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(AzureAuthorityHosts.AzureChina, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsCustomUri_ReturnsCustomUri()
    {
        Dictionary<string, string> initializationInfo = new();
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", MockAuthorityHost);
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(new Uri(MockAuthorityHost), result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenNoEnvironmentVariableIsPresent_AndInitializationInfoIsSet_UsesInitializationInfoValue()
    {
        Dictionary<string, string> initializationInfo = new();
        initializationInfo.Add("AuthorityHost", "public");
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenEnvironmentVariableIsSet_AndInitializatioInfoIsSet_ReturnsValueFromEnvironmentVariable()
    {
        Dictionary<string, string> initializationInfo = new();
        initializationInfo.Add("AuthorityHost", "government");
        
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY_HOST", "public");
        
        Uri result = _factory.GetAzureAuthorityHost(initializationInfo);
        Assert.Equal(AzureAuthorityHosts.AzurePublicCloud, result);
    }
    
    #endregion GetAzureAuthorityHost
    
    #region GetTokenCredentials

    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoIsEmpty_ReturnsDefaultCredentials()
    {
        Dictionary<string, string> initializationInfo = new();

        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials(initializationInfo);
        
        Assert.Equal(typeof(DefaultAzureCredential), result.Value);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoIsMissingAClientSecretField_ReturnsDefaultCredentials()
    {
        Dictionary<string, string> initializationInfo = new();
        initializationInfo.Add("ClientId", "foo");
        initializationInfo.Add("ClientSecret", "bar");

        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials(initializationInfo);
        
        Assert.Equal(typeof(DefaultAzureCredential), result.Value);
    }
    
    [Fact]
    public void GetAzureAuthorityHost_WhenInitializationInfoHasAllClientSecretFields_ReturnsClientSecretCredentials()
    {
        Dictionary<string, string> initializationInfo = new();
        initializationInfo.Add("ClientId", "foo");
        initializationInfo.Add("ClientSecret", "bar");
        initializationInfo.Add("TenantId", "baz");

        KeyValuePair<TokenCredential, Type> result = _factory.GetTokenCredentials(initializationInfo);
        
        Assert.Equal(typeof(ClientSecretCredential), result.Value);
    }
    
    #endregion
}

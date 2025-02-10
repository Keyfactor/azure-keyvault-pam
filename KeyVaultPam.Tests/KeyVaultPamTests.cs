// Copyright 2023 Keyfactor
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

using Azure;
using Azure.Security.KeyVault.Secrets;
using Keyfactor.Extensions.Pam.AzureKeyVault;
using KeyVaultPamTests.Fakes;
using NSubstitute;

namespace KeyVaultPamTests;

public class KeyVaultPamTests
{
    private readonly KeyVaultPam _sut;
    
    private const string MockKeyVaultSecretName = "foo";
    private const string MockKeyVaultSecretValue = "bar";

    public KeyVaultPamTests()
    {
        SecretClient secretClient = Substitute.For<SecretClient>();
        secretClient
            .GetSecretAsync(MockKeyVaultSecretName, Arg.Any<string>())
            .Returns(Task.FromResult(Response.FromValue(new KeyVaultSecret(MockKeyVaultSecretName, MockKeyVaultSecretValue), new FakeAzureResponse())));
        
        _sut = new KeyVaultPam(secretClient);
    }
    
    [Fact]
    public void GetPassword_WhenInvokedWithRequiredParams_ReturnsPassword()
    {
        Dictionary<string, string> instanceParameters = new();
        Dictionary<string, string> initializationInfo = new();
        
        instanceParameters.Add("SecretId", MockKeyVaultSecretName);
        initializationInfo.Add("KeyVaultUri", "https://example.azure.net/");

        string result = _sut.GetPassword(instanceParameters, initializationInfo);
        
        Assert.Equal(MockKeyVaultSecretValue, result);
    }
    
    [Fact]
    public void GetPassword_WhenInvokedWithoutSecretIdParameter_ThrowsException()
    {
        Dictionary<string, string> instanceParameters = new();
        Dictionary<string, string> initializationInfo = new();
        
        initializationInfo.Add("KeyVaultUri", "https://example.azure.net/");

        Assert.Throws<KeyVaultPamException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
    }
    
    [Fact]
    public void GetPassword_WhenInvokedWithEmptySecretIdParameter_ThrowsException()
    {
        Dictionary<string, string> instanceParameters = new();
        Dictionary<string, string> initializationInfo = new();
        
        instanceParameters.Add("SecretId", string.Empty);
        initializationInfo.Add("KeyVaultUri", "https://example.azure.net/");

        Assert.Throws<KeyVaultPamException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
    }
    
    [Fact]
    public void GetPassword_WhenInvokedWithoutKeyVaultUriParameter_ThrowsException()
    {
        Dictionary<string, string> instanceParameters = new();
        Dictionary<string, string> initializationInfo = new();

        // Instantiate KeyVaultPam with default constructor (so it does not use mocked SecretClient)
        KeyVaultPam sut = new ();
        
        instanceParameters.Add("SecretId", MockKeyVaultSecretName);

        Assert.Throws<KeyVaultPamException>(() => sut.GetPassword(instanceParameters, initializationInfo));
    }
    
    [Fact]
    public void GetPassword_WhenInvokedWithEmptyKeyVaultUriParameter_ThrowsException()
    {
        Dictionary<string, string> instanceParameters = new();
        Dictionary<string, string> initializationInfo = new();

        // Instantiate KeyVaultPam with default constructor (so it does not use mocked SecretClient)
        KeyVaultPam sut = new ();
        
        instanceParameters.Add("SecretId", MockKeyVaultSecretName);
        initializationInfo.Add("KeyVaultUri", string.Empty);

        Assert.Throws<KeyVaultPamException>(() => sut.GetPassword(instanceParameters, initializationInfo));
    }
}
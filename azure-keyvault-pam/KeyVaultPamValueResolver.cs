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

using System;
using System.Collections.Generic;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    internal class KeyVaultPamValueResolver
    {
        private readonly ILogger _logger;
        
        public KeyVaultPamValueResolver(ILogger logger)
        {
            _logger = logger;
        }
        
        internal string GetValueFromDictionaryOrEnvironment(Dictionary<string, string> dictionary, string dictionaryName, string dictionaryKey,
            string environmentVariableName)
        {
            _logger.MethodEntry(LogLevel.Debug);
            
            _logger.LogDebug($"Looking up a value for environment variable {environmentVariableName} and for key {dictionaryKey} in dictionary {dictionaryName}...");
            
            string envValue = Environment.GetEnvironmentVariable(environmentVariableName);
            string dictionaryValue = GetValueFromDictionary(dictionary, dictionaryName, dictionaryKey, false);

            string result = null;
            
            if (string.IsNullOrEmpty(envValue) && string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug("No value found in environment variable or dictionary. Defaulting to null.");
            }

            if (string.IsNullOrEmpty(envValue) && !string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug($"Value not found in dictionary but found in dictionary. Returning value from dictionary.");
                result = dictionaryValue;
            }

            if (!string.IsNullOrEmpty(envValue) && !string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug($"Value found in environment variable and dictionary. Returning value from environment variable.");
                result = envValue;
            }
            
            if (!string.IsNullOrEmpty(envValue) && string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug($"Value found in environment variable and not in dictionary. Returning value from environment variable.");
                result = envValue;
            }
            
            _logger.LogDebug($"Finished looking up a value for environment variable {environmentVariableName} and for key {dictionaryKey} in dictionary {dictionaryName}.");
            
            _logger.MethodExit(LogLevel.Debug);

            return result;
        }
        
        internal string GetValueFromDictionary(Dictionary<string, string> dictionary, string dictionaryName, string key, bool throwIfNotFound = true)
        {
            _logger.MethodEntry(LogLevel.Debug);
            _logger.LogDebug($"Getting value of key {key} from dictionary {dictionaryName}...");

            if (!dictionary.ContainsKey(key) || string.IsNullOrEmpty(dictionary[key]))
            {
                string message = $"Dictionary {dictionaryName} is missing a value for {key}";
                
                if (!throwIfNotFound)
                {
                    _logger.LogDebug($"{message}. Returning null");
                    return null;
                }
                
                _logger.LogError(message);
                throw new KeyVaultPamException(message);

            }

            string value = dictionary[key];
            _logger.MethodExit(LogLevel.Debug);
            _logger.LogDebug($"Finished getting value of key {key} from dictionary {dictionaryName}.");

            return value;
        }
    }
}

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

using System;
using System.Collections.Generic;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.AzureKeyVault
{
    /// <summary>
    /// Helper class for resolving values from configuration dictionaries and environment variables.
    /// </summary>
    internal class KeyVaultPamValueResolver
    {
        private readonly ILogger _logger;
        
        public KeyVaultPamValueResolver(ILogger logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Gets a value from either a dictionary or environment variable, with environment variable taking precedence.
        /// Provides a flexible way to retrieve configuration values that can be specified in either source.
        /// If both sources contain a value, the environment variable value is used.
        /// If neither source contains a value, returns null.
        /// </summary>
        /// <param name="dictionary">Source dictionary to look up values</param>
        /// <param name="dictionaryName">Name of dictionary for error messages and logging</param>
        /// <param name="dictionaryKey">Key to look up in the dictionary</param>
        /// <param name="environmentVariableName">Name of environment variable to check first</param>
        /// <returns>The resolved value or null if not found in either location</returns>
        /// <remarks>
        /// The method first checks the environment variable. If found, that value is returned.
        /// Otherwise, it checks the dictionary. If neither contains a value, returns null.
        /// Priority order:
        /// 1. Environment variable value if present
        /// 2. Dictionary value if present
        /// 3. Null if neither is found
        /// </remarks>
        /// <exception cref="KeyVaultPamException">Thrown when environment variable or dictionary value could not be found.</exception>
        internal string GetValueFromDictionaryOrEnvironment(Dictionary<string, string> dictionary, string dictionaryName, string dictionaryKey,
            string environmentVariableName)
        {
            _logger.MethodEntry();
            
            _logger.LogTrace($"Looking up a value for environment variable {environmentVariableName} and for key {dictionaryKey} in dictionary {dictionaryName}...");
            
            string envValue = Environment.GetEnvironmentVariable(environmentVariableName);
            string dictionaryValue = GetValueFromDictionary(dictionary, dictionaryName, dictionaryKey, false);

            string result = null;
            
            if (string.IsNullOrEmpty(envValue) && string.IsNullOrEmpty(dictionaryValue))
            {
                string message = $"No value was found for environment variable {environmentVariableName} or for key {dictionaryKey} in dictionary {dictionaryName}.";
                _logger.LogError(message);
                throw new KeyVaultPamException(message);
            }
            
            if (string.IsNullOrEmpty(envValue) && !string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug($"Value not found in environment but found in dictionary. Returning value from dictionary.");
                result = dictionaryValue;
            }
            else if (!string.IsNullOrEmpty(envValue) && !string.IsNullOrEmpty(dictionaryValue))
            {
                _logger.LogDebug($"Value found in environment variable and dictionary. Returning value from environment variable.");
                result = envValue;
            }
            else 
            {
                _logger.LogDebug($"Value found in environment variable and not in dictionary. Returning value from environment variable.");
                result = envValue;
            }
            
            _logger.LogTrace($"Finished looking up a value for environment variable {environmentVariableName} and for key {dictionaryKey} in dictionary {dictionaryName}.");
            
            _logger.MethodExit();

            return result;
        }
        
        /// <summary>
        /// Gets a value from a dictionary with optional error handling.
        /// </summary>
        /// <param name="dictionary">Source dictionary to look up values</param>
        /// <param name="dictionaryName">Name of dictionary for error messages and logging</param>
        /// <param name="key">Key to look up</param>
        /// <param name="throwIfNotFound">Whether to throw an exception if the key is not found or empty</param>
        /// <returns>The value from the dictionary or null if not found and throwIfNotFound is false</returns>
        /// <remarks>
        /// The method performs the following checks:
        /// 1. Validates that the dictionary parameter is not null
        /// 2. Checks if the key exists in the dictionary
        /// 3. Verifies the value is not null or empty
        /// 
        /// If throwIfNotFound is true (default):
        /// - Throws KeyVaultPamException when key is missing or value is empty
        /// If throwIfNotFound is false:
        /// - Returns null when key is missing or value is empty
        /// </remarks>
        /// <exception cref="KeyVaultPamException">Thrown when key is not found or value is empty, and throwIfNotFound is true</exception>
        internal string GetValueFromDictionary(Dictionary<string, string> dictionary, string dictionaryName, string key, bool throwIfNotFound = true)
        {
            _logger.MethodEntry();
            _logger.LogTrace($"Getting value of key {key} from dictionary {dictionaryName}...");

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
            _logger.MethodExit();
            _logger.LogTrace($"Finished getting value of key {key} from dictionary {dictionaryName}.");

            return value;
        }
    }
}

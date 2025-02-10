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

using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Core;

namespace KeyVaultPamTests.Fakes;

public sealed class FakeAzureResponse : Response
{
    public override int Status => throw new NotImplementedException();

    public override string ReasonPhrase => throw new NotImplementedException();

    public override Stream? ContentStream
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public override string ClientRequestId
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override void Dispose() =>
        throw new NotImplementedException();
    protected override bool ContainsHeader(string name) =>
        throw new NotImplementedException();
    protected override IEnumerable<HttpHeader> EnumerateHeaders() =>
        throw new NotImplementedException();
    protected override bool TryGetHeader(
        string name,
        [NotNullWhen(true)] out string? value) =>
        throw new NotImplementedException();
    protected override bool TryGetHeaderValues(
        string name,
        [NotNullWhen(true)] out IEnumerable<string>? values) =>
        throw new NotImplementedException();
}

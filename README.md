# FileReader Library

Simple library that provides basic file, encrypted text, XML, and JSON reading functionality, including encrypted XML/JSON and role-based authorization for XML and text.

## Usage

- `FileReader.ReadAllText(string path)` - reads file contents synchronously.
- `FileReader.ReadAllTextAsync(string path)` - reads file contents asynchronously.
- `FileReader.ReadAllTextAuthorized(string path, string role, ITextAccessAuthorizer authorizer)` - reads text if authorized for the given role.
- `FileReader.ReadAllTextAuthorizedAsync(string path, string role, ITextAccessAuthorizer authorizer)` - asynchronously reads text if authorized.
- `FileReader.ReadXml(string path)` - reads and parses an XML file into `XDocument`.
- `FileReader.ReadXmlAsync(string path)` - asynchronously reads and parses an XML file into `XDocument`.
- `FileReader.ReadEncryptedText(string path, ITextDecryptor decryptor)` - reads and decrypts an encrypted text file using the provided decryptor.
- `FileReader.ReadEncryptedTextAsync(string path, ITextDecryptor decryptor)` - asynchronously reads and decrypts an encrypted text file using the provided decryptor.
- `FileReader.ReadEncryptedXml(string path, ITextDecryptor decryptor)` - reads, decrypts, and parses an encrypted XML file.
- `FileReader.ReadEncryptedXmlAsync(string path, ITextDecryptor decryptor)` - asynchronously reads, decrypts, and parses an encrypted XML file.
- `FileReader.ReadJson(string path)` - reads and parses a JSON file into `JsonDocument`.
- `FileReader.ReadJsonAsync(string path)` - asynchronously reads and parses a JSON file into `JsonDocument`.
- `FileReader.ReadEncryptedJson(string path, ITextDecryptor decryptor)` - reads, decrypts, and parses an encrypted JSON file.
- `FileReader.ReadEncryptedJsonAsync(string path, ITextDecryptor decryptor)` - asynchronously reads, decrypts, and parses an encrypted JSON file.

## Pluggable decryption

Provide any implementation of `ITextDecryptor`. A sample `ReverseTextDecryptor` is included which simply reverses the text.

## Role-based authorization

- XML: provide any implementation of `IXmlAccessAuthorizer`. A sample `SimpleRoleXmlAccessAuthorizer` is included where `admin` can read any path and other roles must be explicitly allowed.
- Text: provide any implementation of `ITextAccessAuthorizer`. A sample `SimpleRoleTextAccessAuthorizer` is included with the same behavior.

## Run tests

From repository root run:

```
dotnet test
```

## Versioning

- v1.0.0: Initial implementation for reading text files.
- v2.0.0: Adds XML reading (sync/async) with tests.
- v3.0.0: Adds encrypted text reading with pluggable decryptor and tests.
- v4.0.0: Adds role-based authorization for XML reading with pluggable authorizer and tests.
- v5.0.0: Adds encrypted XML reading (sync/async) with pluggable decryptor and tests.
- v6.0.0: Adds role-based authorization for text reading with pluggable authorizer and tests.
- v7.0.0: Adds JSON reading (sync/async) with tests.
- v8.0.0: Adds encrypted JSON reading (sync/async) with pluggable decryptor and tests.
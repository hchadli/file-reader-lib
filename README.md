# FileReader Library

Simple library that provides basic file, encrypted text, and XML reading functionality, including encrypted XML and role-based authorization for XML.

## Usage

- `FileReader.ReadAllText(string path)` - reads file contents synchronously.
- `FileReader.ReadAllTextAsync(string path)` - reads file contents asynchronously.
- `FileReader.ReadXml(string path)` - reads and parses an XML file into `XDocument`.
- `FileReader.ReadXmlAsync(string path)` - asynchronously reads and parses an XML file into `XDocument`.
- `FileReader.ReadEncryptedText(string path, ITextDecryptor decryptor)` - reads and decrypts an encrypted text file using the provided decryptor.
- `FileReader.ReadEncryptedTextAsync(string path, ITextDecryptor decryptor)` - asynchronously reads and decrypts an encrypted text file using the provided decryptor.
- `FileReader.ReadEncryptedXml(string path, ITextDecryptor decryptor)` - reads, decrypts, and parses an encrypted XML file.
- `FileReader.ReadEncryptedXmlAsync(string path, ITextDecryptor decryptor)` - asynchronously reads, decrypts, and parses an encrypted XML file.
- `FileReader.ReadXmlAuthorized(string path, string role, IXmlAccessAuthorizer authorizer)` - reads XML if authorized for the given role.
- `FileReader.ReadXmlAuthorizedAsync(string path, string role, IXmlAccessAuthorizer authorizer)` - asynchronously reads XML if authorized.

## Pluggable decryption

Provide any implementation of `ITextDecryptor`. A sample `ReverseTextDecryptor` is included which simply reverses the text.

## Role-based XML authorization

Provide any implementation of `IXmlAccessAuthorizer`. A sample `SimpleRoleXmlAccessAuthorizer` is included where `admin` can read any path and other roles must be explicitly allowed.

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
# FileReader Library

Simple library that provides basic file, encrypted text, and XML reading functionality.

## Usage

- `FileReader.ReadAllText(string path)` - reads file contents synchronously.
- `FileReader.ReadAllTextAsync(string path)` - reads file contents asynchronously.
- `FileReader.ReadXml(string path)` - reads and parses an XML file into `XDocument`.
- `FileReader.ReadXmlAsync(string path)` - asynchronously reads and parses an XML file into `XDocument`.
- `FileReader.ReadEncryptedText(string path, ITextDecryptor decryptor)` - reads and decrypts an encrypted text file using the provided decryptor.
- `FileReader.ReadEncryptedTextAsync(string path, ITextDecryptor decryptor)` - asynchronously reads and decrypts an encrypted text file using the provided decryptor.

## Pluggable decryption

Provide any implementation of `ITextDecryptor`. A sample `ReverseTextDecryptor` is included which simply reverses the text.

## Run tests

From repository root run:

```
dotnet test
```

## Versioning

- v1.0.0: Initial implementation for reading text files.
- v2.0.0: Adds XML reading (sync/async) with tests.
- v3.0.0: Adds encrypted text reading with pluggable decryptor and tests.
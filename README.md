# FileReader Library

Simple library that provides basic file and XML reading functionality.

## Usage

- `FileReader.ReadAllText(string path)` - reads file contents synchronously.
- `FileReader.ReadAllTextAsync(string path)` - reads file contents asynchronously.
- `FileReader.ReadXml(string path)` - reads and parses an XML file into `XDocument`.
- `FileReader.ReadXmlAsync(string path)` - asynchronously reads and parses an XML file into `XDocument`.

## Run tests

From repository root run:

```
dotnet test
```

## Versioning

- v1.0.0: Initial implementation for reading text files.
- v2.0.0: Adds XML reading (sync/async) with tests.
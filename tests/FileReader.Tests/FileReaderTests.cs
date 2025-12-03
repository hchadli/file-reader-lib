using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace FileReader.Tests
{
    public class FileReaderTests : IDisposable
    {
        private readonly string _tempFilePath;

        public FileReaderTests()
        {
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"filereader_test_{Guid.NewGuid()}.txt");
        }

        [Fact]
        public void ReadAllText_ReturnsContents_WhenFileExists()
        {
            var expected = "Hello, world!";
            File.WriteAllText(_tempFilePath, expected);

            var reader = new FileReaderLibrary.FileReader();
            var actual = reader.ReadAllText(_tempFilePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ReadAllTextAsync_ReturnsContents_WhenFileExists()
        {
            var expected = "Async hello";
            await File.WriteAllTextAsync(_tempFilePath, expected);

            var reader = new FileReaderLibrary.FileReader();
            var actual = await reader.ReadAllTextAsync(_tempFilePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadAllText_ThrowsArgumentNullException_OnNullPath()
        {
            var reader = new FileReaderLibrary.FileReader();
            Assert.Throws<ArgumentNullException>(() => reader.ReadAllText(null!));
        }

        [Fact]
        public async Task ReadAllTextAsync_ThrowsArgumentNullException_OnNullPath()
        {
            var reader = new FileReaderLibrary.FileReader();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await reader.ReadAllTextAsync(null!));
        }

        [Fact]
        public void ReadAllText_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadAllText(missing));
        }

        [Fact]
        public async Task ReadAllTextAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadAllTextAsync(missing));
        }

        [Fact]
        public void ReadXml_ReturnsXDocument_WhenValidXml()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_{Guid.NewGuid()}.xml");
            var xml = "<root><message>Hello</message></root>";
            File.WriteAllText(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var doc = reader.ReadXml(xmlPath);

            Assert.Equal("root", doc.Root!.Name.LocalName);
            Assert.Equal("Hello", doc.Root!.Element("message")!.Value);

            File.Delete(xmlPath);
        }

        [Fact]
        public async Task ReadXmlAsync_ReturnsXDocument_WhenValidXml()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_{Guid.NewGuid()}.xml");
            var xml = "<root><message>Async</message></root>";
            await File.WriteAllTextAsync(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var doc = await reader.ReadXmlAsync(xmlPath);

            Assert.Equal("root", doc.Root!.Name.LocalName);
            Assert.Equal("Async", doc.Root!.Element("message")!.Value);

            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadXml_ThrowsArgumentNullException_OnNullPath()
        {
            var reader = new FileReaderLibrary.FileReader();
            Assert.Throws<ArgumentNullException>(() => reader.ReadXml(null!));
        }

        [Fact]
        public async Task ReadXmlAsync_ThrowsArgumentNullException_OnNullPath()
        {
            var reader = new FileReaderLibrary.FileReader();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await reader.ReadXmlAsync(null!));
        }

        [Fact]
        public void ReadXml_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadXml(missing));
        }

        [Fact]
        public async Task ReadXmlAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadXmlAsync(missing));
        }

        [Fact]
        public void ReadXml_Throws_OnInvalidXml()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_invalid_{Guid.NewGuid()}.xml");
            var invalid = "<root><unclosed>";
            File.WriteAllText(xmlPath, invalid);

            var reader = new FileReaderLibrary.FileReader();
            Assert.ThrowsAny<Exception>(() => reader.ReadXml(xmlPath));

            File.Delete(xmlPath);
        }

        [Fact]
        public async Task ReadXmlAsync_Throws_OnInvalidXml()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_invalid_{Guid.NewGuid()}.xml");
            var invalid = "<root><unclosed>";
            await File.WriteAllTextAsync(xmlPath, invalid);

            var reader = new FileReaderLibrary.FileReader();
            await Assert.ThrowsAsync<System.Xml.XmlException>(() => reader.ReadXmlAsync(xmlPath));

            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadEncryptedText_ReturnsPlainText_WithReverseDecryptor()
        {
            var plain = "Secret";
            var cipher = new string(plain.Reverse().ToArray());
            File.WriteAllText(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var result = reader.ReadEncryptedText(_tempFilePath, decryptor);

            Assert.Equal(plain, result);
        }

        [Fact]
        public async Task ReadEncryptedTextAsync_ReturnsPlainText_WithReverseDecryptor()
        {
            var plain = "AsyncSecret";
            var cipher = new string(plain.Reverse().ToArray());
            await File.WriteAllTextAsync(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var result = await reader.ReadEncryptedTextAsync(_tempFilePath, decryptor);

            Assert.Equal(plain, result);
        }

        [Fact]
        public void ReadEncryptedText_ThrowsArgumentNullException_OnNullDecryptor()
        {
            var reader = new FileReaderLibrary.FileReader();
            File.WriteAllText(_tempFilePath, "abc");
            Assert.Throws<ArgumentNullException>(() => reader.ReadEncryptedText(_tempFilePath, null!));
        }

        [Fact]
        public async Task ReadEncryptedTextAsync_ThrowsArgumentNullException_OnNullDecryptor()
        {
            var reader = new FileReaderLibrary.FileReader();
            await File.WriteAllTextAsync(_tempFilePath, "abc");
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await reader.ReadEncryptedTextAsync(_tempFilePath, null!));
        }

        [Fact]
        public void ReadEncryptedText_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadEncryptedText(missing, decryptor));
        }

        [Fact]
        public async Task ReadEncryptedTextAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadEncryptedTextAsync(missing, decryptor));
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);
            }
            catch
            {
                // ignore cleanup failures
            }
        }
    }
}

using System;
using System.IO;
using System.Text.Json;
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

        [Fact]
        public void ReadXmlAuthorized_AllowsAdmin_ForAnyPath()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_auth_{Guid.NewGuid()}.xml");
            var xml = "<root><message>Admin</message></root>";
            File.WriteAllText(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleXmlAccessAuthorizer();

            var doc = reader.ReadXmlAuthorized(xmlPath, "admin", authorizer);

            Assert.Equal("Admin", doc.Root!.Element("message")!.Value);
            File.Delete(xmlPath);
        }

        [Fact]
        public async Task ReadXmlAuthorizedAsync_AllowsAdmin_ForAnyPath()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_auth_{Guid.NewGuid()}.xml");
            var xml = "<root><message>AdminAsync</message></root>";
            await File.WriteAllTextAsync(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleXmlAccessAuthorizer();

            var doc = await reader.ReadXmlAuthorizedAsync(xmlPath, "admin", authorizer);

            Assert.Equal("AdminAsync", doc.Root!.Element("message")!.Value);
            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadXmlAuthorized_AllowsUser_ForAllowedPath()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_user_{Guid.NewGuid()}.xml");
            var xml = "<root><message>User</message></root>";
            File.WriteAllText(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleXmlAccessAuthorizer(new[] { xmlPath });

            var doc = reader.ReadXmlAuthorized(xmlPath, "user", authorizer);

            Assert.Equal("User", doc.Root!.Element("message")!.Value);
            File.Delete(xmlPath);
        }

        [Fact]
        public async Task ReadXmlAuthorizedAsync_DeniesUser_ForNotAllowedPath()
        {
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_userdeny_{Guid.NewGuid()}.xml");
            var xml = "<root><message>Deny</message></root>";
            await File.WriteAllTextAsync(xmlPath, xml);

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleXmlAccessAuthorizer(new[] { "some-other-path.xml" });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await reader.ReadXmlAuthorizedAsync(xmlPath, "user", authorizer));
            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadXmlAuthorized_ThrowsArgumentNullException_OnNullAuthorizer()
        {
            var reader = new FileReaderLibrary.FileReader();
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_nullauth_[Guid.NewGuid()].xml");
            File.WriteAllText(xmlPath, "<root/>");
            Assert.Throws<ArgumentNullException>(() => reader.ReadXmlAuthorized(xmlPath, "user", null!));
            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadXmlAuthorized_ThrowsArgumentNullException_OnNullRole()
        {
            var reader = new FileReaderLibrary.FileReader();
            var xmlPath = Path.Combine(Path.GetTempPath(), $"filereader_xml_nullrole_[Guid.NewGuid()].xml");
            File.WriteAllText(xmlPath, "<root/>");
            var authorizer = new FileReaderLibrary.SimpleRoleXmlAccessAuthorizer(new[] { xmlPath });
            Assert.Throws<ArgumentNullException>(() => reader.ReadXmlAuthorized(xmlPath, null!, authorizer));
            File.Delete(xmlPath);
        }

        [Fact]
        public void ReadEncryptedXml_ReturnsXDocument_WithReverseDecryptor()
        {
            var plainXml = "<root><message>EncXml</message></root>";
            var cipher = new string(plainXml.Reverse().ToArray());
            File.WriteAllText(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var doc = reader.ReadEncryptedXml(_tempFilePath, decryptor);

            Assert.Equal("EncXml", doc.Root!.Element("message")!.Value);
        }

        [Fact]
        public async Task ReadEncryptedXmlAsync_ReturnsXDocument_WithReverseDecryptor()
        {
            var plainXml = "<root><message>EncXmlAsync</message></root>";
            var cipher = new string(plainXml.Reverse().ToArray());
            await File.WriteAllTextAsync(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var doc = await reader.ReadEncryptedXmlAsync(_tempFilePath, decryptor);

            Assert.Equal("EncXmlAsync", doc.Root!.Element("message")!.Value);
        }

        [Fact]
        public void ReadEncryptedXml_Throws_OnInvalidDecryptedXml()
        {
            var invalidPlain = "<root><oops>";
            var cipher = new string(invalidPlain.Reverse().ToArray());
            File.WriteAllText(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            Assert.ThrowsAny<Exception>(() => reader.ReadEncryptedXml(_tempFilePath, decryptor));
        }

        [Fact]
        public async Task ReadEncryptedXmlAsync_Throws_OnInvalidDecryptedXml()
        {
            var invalidPlain = "<root><oops>";
            var cipher = new string(invalidPlain.Reverse().ToArray());
            await File.WriteAllTextAsync(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            await Assert.ThrowsAsync<System.Xml.XmlException>(async () => await reader.ReadEncryptedXmlAsync(_tempFilePath, decryptor));
        }

        [Fact]
        public void ReadEncryptedXml_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadEncryptedXml(missing, decryptor));
        }

        [Fact]
        public async Task ReadEncryptedXmlAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadEncryptedXmlAsync(missing, decryptor));
        }

        [Fact]
        public void ReadAllTextAuthorized_AllowsAdmin_ForAnyPath()
        {
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_admin_{Guid.NewGuid()}.txt");
            File.WriteAllText(textPath, "admin-ok");

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleTextAccessAuthorizer();

            var content = reader.ReadAllTextAuthorized(textPath, "admin", authorizer);
            Assert.Equal("admin-ok", content);
            File.Delete(textPath);
        }

        [Fact]
        public async Task ReadAllTextAuthorizedAsync_AllowsAdmin_ForAnyPath()
        {
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_admin_async_{Guid.NewGuid()}.txt");
            await File.WriteAllTextAsync(textPath, "admin-async-ok");

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleTextAccessAuthorizer();

            var content = await reader.ReadAllTextAuthorizedAsync(textPath, "admin", authorizer);
            Assert.Equal("admin-async-ok", content);
            File.Delete(textPath);
        }

        [Fact]
        public void ReadAllTextAuthorized_AllowsUser_ForAllowedPath()
        {
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_user_{Guid.NewGuid()}.txt");
            File.WriteAllText(textPath, "user-ok");

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleTextAccessAuthorizer(new[] { textPath });

            var content = reader.ReadAllTextAuthorized(textPath, "user", authorizer);
            Assert.Equal("user-ok", content);
            File.Delete(textPath);
        }

        [Fact]
        public async Task ReadAllTextAuthorizedAsync_DeniesUser_ForNotAllowedPath()
        {
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_userdeny_{Guid.NewGuid()}.txt");
            await File.WriteAllTextAsync(textPath, "deny");

            var reader = new FileReaderLibrary.FileReader();
            var authorizer = new FileReaderLibrary.SimpleRoleTextAccessAuthorizer(new[] { "some-other-path.txt" });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await reader.ReadAllTextAuthorizedAsync(textPath, "user", authorizer));
            File.Delete(textPath);
        }

        [Fact]
        public void ReadAllTextAuthorized_ThrowsArgumentNullException_OnNullAuthorizer()
        {
            var reader = new FileReaderLibrary.FileReader();
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_nullauth_{Guid.NewGuid()}.txt");
            File.WriteAllText(textPath, "data");
            Assert.Throws<ArgumentNullException>(() => reader.ReadAllTextAuthorized(textPath, "user", null!));
            File.Delete(textPath);
        }

        [Fact]
        public void ReadAllTextAuthorized_ThrowsArgumentNullException_OnNullRole()
        {
            var reader = new FileReaderLibrary.FileReader();
            var textPath = Path.Combine(Path.GetTempPath(), $"filereader_text_nullrole_{Guid.NewGuid()}.txt");
            File.WriteAllText(textPath, "data");
            var authorizer = new FileReaderLibrary.SimpleRoleTextAccessAuthorizer(new[] { textPath });
            Assert.Throws<ArgumentNullException>(() => reader.ReadAllTextAuthorized(textPath, null!, authorizer));
            File.Delete(textPath);
        }

        [Fact]
        public void ReadJson_ReturnsDocument_WhenValidJson()
        {
            var jsonPath = Path.Combine(Path.GetTempPath(), $"filereader_json_{Guid.NewGuid()}.json");
            var json = "{\"message\":\"Hello\"}";
            File.WriteAllText(jsonPath, json);

            var reader = new FileReaderLibrary.FileReader();
            using var doc = reader.ReadJson(jsonPath);

            Assert.Equal("Hello", doc.RootElement.GetProperty("message").GetString());
            File.Delete(jsonPath);
        }

        [Fact]
        public async Task ReadJsonAsync_ReturnsDocument_WhenValidJson()
        {
            var jsonPath = Path.Combine(Path.GetTempPath(), $"filereader_json_{Guid.NewGuid()}.json");
            var json = "{\"message\":\"Async\"}";
            await File.WriteAllTextAsync(jsonPath, json);

            var reader = new FileReaderLibrary.FileReader();
            using var doc = await reader.ReadJsonAsync(jsonPath);

            Assert.Equal("Async", doc.RootElement.GetProperty("message").GetString());
            File.Delete(jsonPath);
        }

        [Fact]
        public void ReadJson_Throws_OnInvalidJson()
        {
            var jsonPath = Path.Combine(Path.GetTempPath(), $"filereader_json_invalid_{Guid.NewGuid()}.json");
            File.WriteAllText(jsonPath, "{invalid}");

            var reader = new FileReaderLibrary.FileReader();
            Assert.ThrowsAny<Exception>(() => reader.ReadJson(jsonPath));
            File.Delete(jsonPath);
        }

        [Fact]
        public async Task ReadJsonAsync_Throws_OnInvalidJson()
        {
            var jsonPath = Path.Combine(Path.GetTempPath(), $"filereader_json_invalid_{Guid.NewGuid()}.json");
            await File.WriteAllTextAsync(jsonPath, "{invalid}");

            var reader = new FileReaderLibrary.FileReader();
            await Assert.ThrowsAsync<Exception>(async () => await reader.ReadJsonAsync(jsonPath));
            File.Delete(jsonPath);
        }

        [Fact]
        public void ReadJson_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadJson(missing));
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadJsonAsync(missing));
        }

        [Fact]
        public void ReadEncryptedJson_ReturnsDocument_WithReverseDecryptor()
        {
            var plainJson = "{\"message\":\"EncJson\"}";
            var cipher = new string(plainJson.Reverse().ToArray());
            File.WriteAllText(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            using var doc = reader.ReadEncryptedJson(_tempFilePath, decryptor);

            Assert.Equal("EncJson", doc.RootElement.GetProperty("message").GetString());
        }

        [Fact]
        public async Task ReadEncryptedJsonAsync_ReturnsDocument_WithReverseDecryptor()
        {
            var plainJson = "{\"message\":\"EncJsonAsync\"}";
            var cipher = new string(plainJson.Reverse().ToArray());
            await File.WriteAllTextAsync(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            using var doc = await reader.ReadEncryptedJsonAsync(_tempFilePath, decryptor);

            Assert.Equal("EncJsonAsync", doc.RootElement.GetProperty("message").GetString());
        }

        [Fact]
        public void ReadEncryptedJson_Throws_OnInvalidDecryptedJson()
        {
            var invalidPlain = "{invalid}";
            var cipher = new string(invalidPlain.Reverse().ToArray());
            File.WriteAllText(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            Assert.ThrowsAny<Exception>(() => reader.ReadEncryptedJson(_tempFilePath, decryptor));
        }

        [Fact]
        public async Task ReadEncryptedJsonAsync_Throws_OnInvalidDecryptedJson()
        {
            var invalidPlain = "{invalid}";
            var cipher = new string(invalidPlain.Reverse().ToArray());
            await File.WriteAllTextAsync(_tempFilePath, cipher);

            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            await Assert.ThrowsAsync<Exception>(async () => await reader.ReadEncryptedJsonAsync(_tempFilePath, decryptor));
        }

        [Fact]
        public void ReadEncryptedJson_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.Throws<FileNotFoundException>(() => reader.ReadEncryptedJson(missing, decryptor));
        }

        [Fact]
        public async Task ReadEncryptedJsonAsync_ThrowsFileNotFound_WhenMissing()
        {
            var reader = new FileReaderLibrary.FileReader();
            var decryptor = new FileReaderLibrary.ReverseTextDecryptor();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<FileNotFoundException>(async () => await reader.ReadEncryptedJsonAsync(missing, decryptor));
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

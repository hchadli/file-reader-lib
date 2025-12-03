using System;
using System.IO;
using System.Threading.Tasks;
using FileReaderLibrary;
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

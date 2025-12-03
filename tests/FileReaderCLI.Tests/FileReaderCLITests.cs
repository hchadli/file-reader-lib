using System.Diagnostics;
using System.Text;
using System.Linq;
using Xunit;

namespace FileReaderCLI.Tests
{
    public class FileReaderCLITests
    {
        private static string GetSolutionRoot()
        {
            var baseDir = AppContext.BaseDirectory; // .../tests/FileReaderCLI.Tests/bin/Debug/net10.0/
            return Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));
        }

        private static string GetCliProjectPath()
        {
            var solutionRoot = GetSolutionRoot();
            return Path.Combine(solutionRoot, "src", "FileReaderCLI", "FileReaderCLI.csproj");
        }

        private static (int ExitCode, string Output) RunCliWithInput(string input)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{GetCliProjectPath()}\" -c Debug",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = GetSolutionRoot()
            };

            using var proc = Process.Start(psi)!;
            proc.StandardInput.Write(input);
            proc.StandardInput.Flush();
            proc.StandardInput.Close();
            var output = proc.StandardOutput.ReadToEnd();
            var error = proc.StandardError.ReadToEnd();
            proc.WaitForExit(30000);
            var combined = output + (string.IsNullOrEmpty(error) ? string.Empty : "\nERROR:\n" + error);
            return (proc.ExitCode, combined);
        }

        [Fact]
        public void Cli_ReadText_HappyPath()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cli_text_{Guid.NewGuid()}.txt");
            var content = "hello-cli";
            File.WriteAllText(path, content);

            // Sequence: 1 (Text), path, n (encryption), n (auth), n (exit)
            var input = new StringBuilder()
                .AppendLine("1")
                .AppendLine(path)
                .AppendLine("n")
                .AppendLine("n")
                .AppendLine("n")
                .ToString();

            var (code, output) = RunCliWithInput(input);

            Assert.Equal(0, code);
            Assert.Contains("--- TEXT CONTENT START ---", output);
            Assert.Contains(content, output);
            Assert.Contains("--- TEXT CONTENT END ---", output);

            File.Delete(path);
        }

        [Fact]
        public void Cli_ReadXml_WithAuthorization_HappyPath()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cli_xml_{Guid.NewGuid()}.xml");
            var xml = "<root><message>ok</message></root>";
            File.WriteAllText(path, xml);

            // Sequence: 2 (XML), path, n (encryption), y (auth), admin (role), n (exit)
            var input = new StringBuilder()
                .AppendLine("2")
                .AppendLine(path)
                .AppendLine("n")
                .AppendLine("y")
                .AppendLine("admin")
                .AppendLine("n")
                .ToString();

            var (code, output) = RunCliWithInput(input);

            Assert.Equal(0, code);
            Assert.Contains("--- XML CONTENT START ---", output);
            Assert.Contains("<message>ok</message>", output);
            Assert.Contains("--- XML CONTENT END ---", output);

            File.Delete(path);
        }

        [Fact]
        public void Cli_ReadJson_WithEncryption_HappyPath()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cli_json_{Guid.NewGuid()}.json");
            var plain = "{\"message\":\"okjson\"}";
            var cipher = new string(plain.Reverse().ToArray());
            File.WriteAllText(path, cipher);

            // Sequence: 3 (JSON), path, y (encryption), n (auth), n (exit)
            var input = new StringBuilder()
                .AppendLine("3")
                .AppendLine(path)
                .AppendLine("y")
                .AppendLine("n")
                .AppendLine("n")
                .ToString();

            var (code, output) = RunCliWithInput(input);

            Assert.Equal(0, code);
            Assert.Contains("--- JSON CONTENT START ---", output);
            Assert.Contains("\"message\": \"okjson\"", output);
            Assert.Contains("--- JSON CONTENT END ---", output);

            File.Delete(path);
        }
    }
}

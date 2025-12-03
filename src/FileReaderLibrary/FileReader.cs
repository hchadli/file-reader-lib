using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileReaderLibrary
{
    /// <summary>
    /// Simple file reading helper.
    /// </summary>
    public class FileReader
    {
        /// <summary>
        /// Reads all text from a file.
        /// </summary>
        /// <param name="path">Path to the text file.</param>
        /// <returns>File contents as string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">If file does not exist.</exception>
        public string ReadAllText(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            return File.ReadAllText(path);
        }

        /// <summary>
        /// Asynchronously reads all text from a file.
        /// </summary>
        /// <param name="path">Path to the text file.</param>
        /// <returns>File contents as string.</returns>
        public async Task<string> ReadAllTextAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            using var sr = new StreamReader(path);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads and parses an XML file into an XDocument.
        /// </summary>
        public XDocument ReadXml(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            using var fs = File.OpenRead(path);
            return XDocument.Load(fs);
        }

        /// <summary>
        /// Asynchronously reads and parses an XML file into an XDocument.
        /// </summary>
        public async Task<XDocument> ReadXmlAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return await Task.Run(() => XDocument.Load(fs)).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads an encrypted text file then decrypts using the provided decryptor.
        /// </summary>
        public string ReadEncryptedText(string path, ITextDecryptor decryptor)
        {
            if (decryptor == null) throw new ArgumentNullException(nameof(decryptor));
            var cipher = ReadAllText(path);
            return decryptor.Decrypt(cipher);
        }

        /// <summary>
        /// Asynchronously reads an encrypted text file then decrypts using the provided decryptor.
        /// </summary>
        public async Task<string> ReadEncryptedTextAsync(string path, ITextDecryptor decryptor)
        {
            if (decryptor == null) throw new ArgumentNullException(nameof(decryptor));
            var cipher = await ReadAllTextAsync(path).ConfigureAwait(false);
            return await decryptor.DecryptAsync(cipher).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads an XML file if authorized by the provided authorizer.
        /// </summary>
        public XDocument ReadXmlAuthorized(string path, string role, IXmlAccessAuthorizer authorizer)
        {
            if (authorizer == null) throw new ArgumentNullException(nameof(authorizer));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));
            if (!authorizer.CanRead(path, role))
                throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.");

            return ReadXml(path);
        }

        /// <summary>
        /// Asynchronously reads an XML file if authorized by the provided authorizer.
        /// </summary>
        public async Task<XDocument> ReadXmlAuthorizedAsync(string path, string role, IXmlAccessAuthorizer authorizer)
        {
            if (authorizer == null) throw new ArgumentNullException(nameof(authorizer));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));
            if (!authorizer.CanRead(path, role))
                throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.");

            return await ReadXmlAsync(path).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads an encrypted XML file and parses it using the provided decryptor.
        /// </summary>
        public XDocument ReadEncryptedXml(string path, ITextDecryptor decryptor)
        {
            if (decryptor == null) throw new ArgumentNullException(nameof(decryptor));
            var cipher = ReadAllText(path);
            var plain = decryptor.Decrypt(cipher);
            return XDocument.Parse(plain);
        }

        /// <summary>
        /// Asynchronously reads an encrypted XML file and parses it using the provided decryptor.
        /// </summary>
        public async Task<XDocument> ReadEncryptedXmlAsync(string path, ITextDecryptor decryptor)
        {
            if (decryptor == null) throw new ArgumentNullException(nameof(decryptor));
            var cipher = await ReadAllTextAsync(path).ConfigureAwait(false);
            var plain = await decryptor.DecryptAsync(cipher).ConfigureAwait(false);
            return XDocument.Parse(plain);
        }

        /// <summary>
        /// Reads a text file if authorized by the provided authorizer.
        /// </summary>
        public string ReadAllTextAuthorized(string path, string role, ITextAccessAuthorizer authorizer)
        {
            if (authorizer == null) throw new ArgumentNullException(nameof(authorizer));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));
            if (!authorizer.CanRead(path, role))
                throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.");

            return ReadAllText(path);
        }

        /// <summary>
        /// Asynchronously reads a text file if authorized by the provided authorizer.
        /// </summary>
        public async Task<string> ReadAllTextAuthorizedAsync(string path, string role, ITextAccessAuthorizer authorizer)
        {
            if (authorizer == null) throw new ArgumentNullException(nameof(authorizer));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));
            if (!authorizer.CanRead(path, role))
                throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.");

            return await ReadAllTextAsync(path).ConfigureAwait(false);
        }
    }
}

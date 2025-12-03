using System;
using System.IO;
using System.Threading.Tasks;

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
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileReaderLibrary
{
    /// <summary>
    /// Simple decryptor that reverses the text.
    /// </summary>
    public class ReverseTextDecryptor : ITextDecryptor
    {
        public string Decrypt(string cipherText)
        {
            if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));
            var chars = cipherText.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public Task<string> DecryptAsync(string cipherText)
        {
            return Task.FromResult(Decrypt(cipherText));
        }
    }
}

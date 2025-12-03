namespace FileReaderLibrary
{
    public interface ITextDecryptor
    {
        string Decrypt(string cipherText);
        Task<string> DecryptAsync(string cipherText);
    }
}

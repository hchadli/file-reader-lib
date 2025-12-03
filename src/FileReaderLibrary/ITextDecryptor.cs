namespace FileReaderLibrary
{
    public interface ITextDecryptor
    {
        string Decrypt(string cipherText);
        System.Threading.Tasks.Task<string> DecryptAsync(string cipherText);
    }
}

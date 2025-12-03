namespace FileReaderLibrary
{
    public interface ITextAccessAuthorizer
    {
        bool CanRead(string path, string role);
    }
}

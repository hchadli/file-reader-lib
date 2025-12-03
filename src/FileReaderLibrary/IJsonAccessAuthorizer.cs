namespace FileReaderLibrary
{
    public interface IJsonAccessAuthorizer
    {
        bool CanRead(string path, string role);
    }
}

namespace FileReaderLibrary
{
    public interface IXmlAccessAuthorizer
    {
        bool CanRead(string path, string role);
    }
}

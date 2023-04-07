namespace MyHomework.Interfaces
{
    public interface IFileProvider
    {
        Task<string> ReadAsync(string fileName);
        Task WriteAsync(string fileName, string content, bool append, CancellationToken cancellationToken);
    }
}
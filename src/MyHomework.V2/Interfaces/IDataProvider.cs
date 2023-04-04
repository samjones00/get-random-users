namespace MyHomework.Interfaces
{
    public interface IDataProvider
    {
        Task<string> ReadAsync(string fileName);
        void WriteAsync(string fileName, string content, bool append, CancellationToken cancellationToken);
    }
}
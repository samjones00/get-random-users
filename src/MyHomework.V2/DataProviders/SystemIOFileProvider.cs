using System.Diagnostics.CodeAnalysis;

namespace MyHomework.Interfaces
{
    [ExcludeFromCodeCoverage]
    public class SystemIOFileProvider : IFileProvider
    {
        public async Task WriteAsync(string fileName, string content, bool append, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(cancellationToken);

            CreateDirectoryIfNotExists(fileName);

            using StreamWriter streamWriter = new(fileName, append);
            await streamWriter.WriteLineAsync(content.ToCharArray(), cancellationToken);
        }

        public async Task<string> ReadAsync(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            using StreamReader reader = new(fileName);
            return await reader.ReadToEndAsync();
        }

        private static void CreateDirectoryIfNotExists(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);

            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
using System.Collections.Concurrent;
using System.Text.Json;

namespace Data
{
    public sealed class Logger(string filePath) : ILogger
    {
        private readonly BlockingCollection<(object Message, DateTime Time)> _queue = [];

        private readonly JsonSerializerOptions _jsonSerializerOptions = new();

        public async Task LoggingThread(CancellationToken token)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to remove log file: {filePath}: {ex}");
                return;
            }

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var (message, time) = _queue.Take(token);
                    var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);

                    await File.AppendAllTextAsync(filePath, $"[{time}] [{message.GetType().Name}] {json}\n", token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public void Log<T>(T? message)
        {
            if (message is null)
                return;

            _queue.Add((message, DateTime.Now));
        }
    }
}

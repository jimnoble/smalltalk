using SmallTalk.Data.Schema;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmallTalk.Data;

class MessageDateRepository : IMessageDateRepository
{
    static Mutex _mutex = new Mutex();
    
    static readonly string _rootPath = "c:/smalltalk/";

    static readonly Encoding _encoding = new UTF8Encoding(false);
    
    public async IAsyncEnumerable<DateTime> EnumerateDatesAsync(string channel)
    {
        var basePath = Path.Combine(_rootPath, $"message-dates/{channel}");

        Directory.CreateDirectory(basePath);

        foreach (var path in Directory.EnumerateFiles(basePath, "*.*", SearchOption.AllDirectories))
        {
            var match = Regex.Match(
                path.Substring(_rootPath.Length).Replace('\\', '/'),
                @"message-dates/(?<Channel>.*?)/(?<Date>.*?)\.jsonl");

            if (match.Success)
            {
                yield return DateTime.Parse(match.Groups["Date"].Value);
            }
        }

        yield break;
    }

    public async IAsyncEnumerable<Message> EnumerateMessagesAsync(
        string channel, 
        DateTime date)
    {
        //var result = new List<Message>();

        var path = Path.Combine(_rootPath, $"message-dates/{channel}/{date:yyyy/MM/dd}.jsonl");

        lock (this)
        {
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream))
            {
                while (true)
                {
                    var line = streamReader.ReadLine();

                    if (line == null) break;

                    var message = JsonSerializer.Deserialize<Message>(line);

                    yield return message;
                    //result.Add(message);
                }
            }
        }

        //return result;
    }

    public Task AppendMessageAsync(
        string channel, 
        DateTime date, 
        Message message)
    {
        var path = Path.Combine(_rootPath, $"message-dates/{channel}/{date:yyyy/MM/dd}.jsonl");

        var payload = $"{JsonSerializer.Serialize(message)}\r\n";

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        lock (this)
        {
            using (var fileStream = File.Open(path, FileMode.Append))
            {
                fileStream.Write(_encoding.GetBytes(payload));
            }
        }

        return Task.CompletedTask;
    }
}

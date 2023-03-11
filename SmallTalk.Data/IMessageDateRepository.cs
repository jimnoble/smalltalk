using SmallTalk.Data.Schema;

namespace SmallTalk.Data;

public interface IMessageDateRepository
{
    IAsyncEnumerable<DateTime> EnumerateDatesAsync(string channel);

    IAsyncEnumerable<Message> EnumerateMessagesAsync(
        string channel,
        DateTime date);

    Task AppendMessageAsync(
        string channel,
        DateTime date,
        Message message);
}

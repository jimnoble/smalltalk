namespace SmallTalk.Data.Schema;

public record MessagePublishedEvent(
    string Channel,
    string Author,
    bool WasEdit);

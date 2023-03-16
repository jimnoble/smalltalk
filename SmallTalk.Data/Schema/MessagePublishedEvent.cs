namespace SmallTalk.Data.Schema;

public record MessagePublishedEvent(
    string Channel,
    bool WasEdit);

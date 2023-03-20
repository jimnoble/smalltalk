namespace SmallTalk.Data.Schema;

public record MessagePublishedEvent(
    string Channel,
    string Author,
    bool WasEdit);

public record UserTypingEvent(
    string Channel,
    string User);

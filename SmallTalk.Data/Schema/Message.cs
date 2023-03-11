namespace SmallTalk.Data.Schema;

public record struct Message(
    Guid MessageId,
    string Channel,
    string Author,
    DateTime WriteTime,
    DateTime? ParentMessageDate,
    Guid? ParentMessageId,
    string Content,
    Attachment[] Attachments,
    Reaction[] Reactions);

public record struct Attachment(
    Guid AttachmentId,
    string Label,
    string ContentType,
    int SizeBytes,
    string Url);

public record struct Reaction(
    string Glyph,
    string[] Authors);


public record struct ChannelDateKey(
    string Channel,
    DateTime? Date);
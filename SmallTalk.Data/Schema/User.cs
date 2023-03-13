namespace SmallTalk.Data.Schema;

public record struct User(
    string Email,
    string DisplayName,
    string AvatarColor,
    string AvatarUrl);

using NodaTime;

namespace SqlQueryBuilder.IntegrationTest;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

public sealed class DbUser {
    public long Id { get; set; }
    public string Username { get; set; }
    public int Age { get; set; }
    public string? Color { get; set; }
    public int Counter { get; set; }
    public long StreetId { get; set; }
    public LocalDateTime CreatedAt { get; set; }
}

public sealed class DbStreet {
    public long Id { get; set; }
    public string Name { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

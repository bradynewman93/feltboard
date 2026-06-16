namespace Common.Models;

public record DiscoveredArticle
{
    public required string Url { get; init; }
    public required string ResourceType { get; init; }
    public required DateTimeOffset DiscoveredAt { get; init; }
}

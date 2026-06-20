using Common.Constants;

namespace Common.Models;

public record DiscoveredArticle
{
    public required string ResourceUrl { get; init; }
    public required string ResourceType { get; init; }
    
    public required string ResourceSource { get; init; }
    public required DateTimeOffset DiscoveredAt { get; init; }

    public string ProcessingStatus = ResProcessingStatus.Queued;
}

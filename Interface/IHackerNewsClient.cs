using SantanderNewsApi.Data;

namespace SantanderNewsApi.Interface;

public interface IHackerNewsClient
{
    Task<int[]> GetBestStoriesAsync(CancellationToken cancellationToken);
    Task<NewsItem?> GetStoryDetailsAsync(int id, CancellationToken cancellationToken);
}

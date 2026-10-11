using SantanderNewsApi.Data;

namespace SantanderNewsApi.Interface;

public interface INewsService
{
    Task<IEnumerable<NewsItem>> GetBestNewsItemsAsync(int count, CancellationToken cancellationToken);
}

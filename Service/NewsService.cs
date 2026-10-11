using Microsoft.Extensions.Caching.Memory;
using SantanderNewsApi.Data;
using SantanderNewsApi.Interface;

namespace SantanderNewsApi.Service;

public class NewsService : INewsService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHackerNewsClient _hackerNewsClient;
    private const string BESTSTORYIDS_CACHEKEY = "BESTSTORYIDS";
    private const int SEMAPHORE_TOTAL = 20;
    private const int BEST_STORIES_CACHE_DURATION_MINS = 1;
    private const int STORY_DETAILS_CACHE_DURATION_MINS = 5;

    public NewsService(IMemoryCache memoryCache, IHackerNewsClient hackerNewsClient)
    {
        _memoryCache = memoryCache;
        _hackerNewsClient = hackerNewsClient;
    }

    public async Task<IEnumerable<NewsItem>> GetBestNewsItemsAsync(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return Enumerable.Empty<NewsItem>();
        }

        int[]? bestStoryIds = await _memoryCache.GetOrCreateAsync(BESTSTORYIDS_CACHEKEY, async (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BEST_STORIES_CACHE_DURATION_MINS);
            return await _hackerNewsClient.GetBestStoriesAsync(cancellationToken);
        });

        if (null == bestStoryIds || 0 == bestStoryIds.Length)
        {
            return Enumerable.Empty<NewsItem>();
        }

        var selectedIds = bestStoryIds.Take(count);
        using var semaphore = new SemaphoreSlim(SEMAPHORE_TOTAL);
        var tasks = selectedIds.Select(async (id) =>
        {
            string cacheKey = $"story_{id}";
            var story = await _memoryCache.GetOrCreateAsync<NewsItem>(cacheKey, async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(STORY_DETAILS_CACHE_DURATION_MINS);
                await semaphore.WaitAsync(cancellationToken);
                try
                {
#pragma warning disable CS8603 // Possible null reference return.
                    return await _hackerNewsClient.GetStoryDetailsAsync(id, cancellationToken);
#pragma warning restore CS8603 // Possible null reference return.
                }
                finally
                {
                    semaphore.Release();
                }
            });
            return story;
        });

        var results = await Task.WhenAll(tasks);

        if (null == results)
        {
            return Enumerable.Empty<NewsItem>();
        }

        return results
            .Where(newsItem => null != newsItem) //filtering out explicitly any null values. (disabled the above compiler warning)
            .Select(newsItem => newsItem!) //Using dammit operator to confirm it is not null.
            .OrderByDescending(newsItem => newsItem.Score);
    }
}

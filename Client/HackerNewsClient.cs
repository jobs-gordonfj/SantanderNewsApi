using SantanderNewsApi.Data;
using SantanderNewsApi.Interface;

namespace SantanderNewsApi.Client;

public class HackerNewsClient : IHackerNewsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsClient> _logger;

    public HackerNewsClient(HttpClient httpClient, ILogger<HackerNewsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int[]> GetBestStoriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var ids = await _httpClient.GetFromJsonAsync<int[]>("beststories.json", cancellationToken);
            if (null == ids)
            {
                return Array.Empty<int>();
            }
            return ids;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetBestStoriesAsync");
            return Array.Empty<int>();
        }
    }

    public async Task<NewsItem?> GetStoryDetailsAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var hackerNewsItem = await _httpClient.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json", cancellationToken);
            if (null == hackerNewsItem)
            {
                return null;
            }
            NewsItem newsItem = new NewsItem()
            {
                Title = hackerNewsItem.Title,
                Uri = hackerNewsItem.Url,
                PostedBy = hackerNewsItem.By,
                Time = DateTimeOffset.FromUnixTimeSeconds(hackerNewsItem.Time),
                Score = hackerNewsItem.Score,
                CommentCount = hackerNewsItem.Descendants
            };
            return newsItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetStoryDetailsAsync for story id: {id}");
            return null;
        }
    }
}

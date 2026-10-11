using System.Text.Json;
using SantanderNewsApi.Client;
using SantanderNewsApi.Interface;
using SantanderNewsApi.Service;

namespace SantanderNewsApi;

public static class Program
{
    private const string HACKER_NEWS_API_BASE_URI = "https://hacker-news.firebaseio.com/v0/";

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.ConfigureHttpJsonOptions((configureOptions) =>
        {
            configureOptions.SerializerOptions.PropertyNameCaseInsensitive = true;
            configureOptions.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
        {
            client.BaseAddress = new Uri(HACKER_NEWS_API_BASE_URI);
        });
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<INewsService, NewsService>();

        var app = builder.Build();
        app.UseHttpsRedirection();
        app.MapGet("/", GetBestStoriesResult); //Added a default result of top 10 (count has default of 10 in function signature) best new stories for "/" request.
        app.MapGet("/v0/beststories/{count}", GetBestStoriesResult); //count gets parsed out of the URI and passed into the function.
        app.Run();
    }

    private static async Task<IResult> GetBestStoriesResult(INewsService newsService, CancellationToken cancellationToken, int count = 10)
    {
        if (count <= 0)
        {
            return Results.BadRequest(new { error = "Count must be an integer greater than zero." });
        }

        var beststories = await newsService.GetBestNewsItemsAsync(count, cancellationToken);
        return Results.Ok(beststories);
    }
}
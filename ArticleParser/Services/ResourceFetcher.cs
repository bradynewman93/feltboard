namespace ArticleParser.Services;

public class ResourceFetcher : IResourceFetcher
{
    private readonly HttpClient _httpClient;

    public ResourceFetcher(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetArticleHtml(string articleUrl)
    {
        int[] retryDelaysSeconds = [5, 15, 30];

        for (int attempt = 0; ; attempt++)
        {
            var response = await _httpClient.GetAsync(articleUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < retryDelaysSeconds.Length)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelaysSeconds[attempt]));
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

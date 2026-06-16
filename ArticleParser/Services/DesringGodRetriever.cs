namespace ArticleParser.Services;

public class DesiringGodRetriever : IArticleRetriever
{
    private readonly HttpClient _httpClient;

    public DesiringGodRetriever(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetArticleHtml(string articleUrl)
    {
        var response = await _httpClient.GetAsync(articleUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
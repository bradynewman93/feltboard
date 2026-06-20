namespace ArticleParser.Services;


public interface IResourceFetcher
{
    public Task<string> GetArticleHtml(string articleUrl);
}
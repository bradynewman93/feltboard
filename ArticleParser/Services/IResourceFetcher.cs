using HtmlAgilityPack;

namespace ArticleParser.Services;


public interface IResourceFetcher
{
    public Task<HtmlDocument> GetArticleHtml(string articleUrl);
}
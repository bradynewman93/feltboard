namespace DesiringGodParser.Services;


public interface IArticleRetriever
{
    public Task<string> GetArticleHtml(string articleUrl);
}
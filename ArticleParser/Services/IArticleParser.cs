using ArticleParser.Models;

namespace ArticleParser.Services;


public interface IArticleParser
{
    public ParsedArticle ParseArticleHtml(string articleUrl, string articleHtml);
}
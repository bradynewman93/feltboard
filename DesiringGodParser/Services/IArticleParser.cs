using DesiringGodParser.Models;

namespace DesiringGodParser.Services;


public interface IArticleParser
{
    public ParsedArticle ParseArticleHtml(string articleHtml);
}
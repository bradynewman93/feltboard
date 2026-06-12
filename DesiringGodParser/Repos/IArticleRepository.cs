using DesiringGodParser.Models;

namespace DesiringGodParser.Repos;


public interface IArticleRepository
{
    public Task SaveArticle(ParsedArticle article);
}
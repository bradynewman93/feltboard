using DesiringGodParser.Models;

namespace DesiringGodParser.Repos;


public interface IArticleRepository
{
    public Task<ParsedArticle> SaveArticle(ParsedArticle article);
}
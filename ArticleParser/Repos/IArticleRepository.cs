using ArticleParser.Models;

namespace ArticleParser.Repos;


public interface IArticleRepository
{
    public Task<ParsedArticle> SaveArticle(ParsedArticle article);
}
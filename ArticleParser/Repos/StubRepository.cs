using ArticleParser.Models;
using ArticleParser.Util;

namespace ArticleParser.Repos;

public class StubRepository : IArticleRepository
{

    public async Task<ParsedArticle> SaveArticle(ParsedArticle article)
    {
        string url = article.Url;

        string key = Sha256Util.GenerateSha256Hash(url);

        article.Key = key;
        return article;
    }
}
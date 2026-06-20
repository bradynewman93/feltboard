using ArticleParser.Models;

namespace ArticleParser.Services;

public class S3KnowledgeBase : IArticleKnowledgeBase
{
    public Task PersistResource(ParsedArticle parsedResource)
    {
        throw new NotImplementedException();
    }
}
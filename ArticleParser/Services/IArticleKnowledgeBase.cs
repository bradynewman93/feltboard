using ArticleParser.Models;
using Common.Models;

namespace ArticleParser.Services;

public interface IArticleKnowledgeBase
{
    public Task PersistResource(ParsedArticle parsedResource);
}
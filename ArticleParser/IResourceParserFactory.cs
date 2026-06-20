using Common.Models;
using ArticleParser.Parsers;

namespace ArticleParser;

public interface IResourceParserFactory
{
    public IResourceParser CreateResourceParser(DiscoveredArticle resource);
}
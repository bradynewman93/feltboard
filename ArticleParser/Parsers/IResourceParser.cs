using System.Text.Json;
using ArticleParser.Models;
using Common.Models;
using HtmlAgilityPack;

namespace ArticleParser.Parsers;

public interface IResourceParser
{
    public Task<ParsedArticle> ParseResource(DiscoveredArticle resource, HtmlDocument resourceHtml);
}
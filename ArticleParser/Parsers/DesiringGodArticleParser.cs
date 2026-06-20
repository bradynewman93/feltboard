using System.Text.RegularExpressions;
using ArticleParser.Models;
using Common.Models;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace ArticleParser.Parsers;

public class DesiringGodArticleParser : IResourceParser
{
    private readonly ILogger _logger;

    public DesiringGodArticleParser(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task<ParsedArticle> ParseResource(DiscoveredArticle resource, HtmlDocument resourceHtml)
    {
        var title = resourceHtml.DocumentNode
                        .SelectSingleNode("//h1[contains(@class,'resource__title')]")?.InnerText.Trim()
                    ?? string.Empty;

        var author = resourceHtml.DocumentNode
                         .SelectSingleNode("//*[contains(@class,'js-modal-author-name')]")?.InnerText.Trim()
                     ?? string.Empty;

        var dateStr = resourceHtml.DocumentNode
            .SelectSingleNode("//time[@datetime]")?.GetAttributeValue("datetime", null);

        // Now remove noise from the body content node only
        var contentNode = resourceHtml.DocumentNode
            .SelectSingleNode("//*[contains(@class,'resource__body')]");

        var bodyText = string.Empty;
        if (contentNode is not null)
        {
            // Remove scripts/styles nested in body
            var bodyNoise = contentNode.SelectNodes(".//script | .//style | .//nav");
            if (bodyNoise is not null)
                foreach (var node in bodyNoise)
                    node.Remove();

            bodyText = HtmlEntity.DeEntitize(contentNode.InnerText);
            bodyText = Regex.Replace(bodyText, @"\s+", " ").Trim();
        }

        return new ParsedArticle
        {
            Url = resource.ResourceUrl,
            Source = resource.ResourceSource,
            Title = title,
            BodyText = bodyText,
            Author = author
        };
    }
}
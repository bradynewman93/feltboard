
using DesiringGodParser.Models;
using DesiringGodParser.Services;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

public class DesiringGodArticleParser : IArticleParser
{

    private const string DesiringGodSource = "DESIRING-GOD";

    public ParsedArticle ParseArticleHtml(string articleUrl, string articleHtml)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(articleHtml);

        // Extract header fields BEFORE removing noise
        var title = doc.DocumentNode
            .SelectSingleNode("//h1[contains(@class,'resource__title')]")?.InnerText.Trim()
            ?? string.Empty;

        var author = doc.DocumentNode
            .SelectSingleNode("//*[contains(@class,'js-modal-author-name')]")?.InnerText.Trim()
            ?? string.Empty;

        var dateStr = doc.DocumentNode
            .SelectSingleNode("//time[@datetime]")?.GetAttributeValue("datetime", null);

        // Now remove noise from the body content node only
        var contentNode = doc.DocumentNode
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
            Url = articleUrl,
            Source = DesiringGodSource,
            Title = title,
            BodyText = bodyText,
            Author = author
        };
    }
}
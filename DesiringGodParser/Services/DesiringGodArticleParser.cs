
using DesiringGodParser.Models;
using DesiringGodParser.Services;

public class DesiringGodArticleParser : IArticleParser
{

    public ParsedArticle ParseArticleHtml(string articleHtml)
    {
        
        //     var doc = new HtmlDocument();
        // doc.LoadHtml(html);

        // // Remove noise nodes first
        // var noiseSelectors = new[]
        // {
        //     "//nav", "//header", "//footer", "//script",
        //     "//style", "//aside", "//*[@class='related-articles']",
        //     "//*[contains(@class,'social')]", "//*[contains(@class,'ad')]"
        // };

        // foreach (var selector in noiseSelectors)
        // {
        //     var nodes = doc.DocumentNode.SelectNodes(selector);
        //     if (nodes is null) continue;
        //     foreach (var node in nodes)
        //         node.Remove();
        // }

        // // TGC-specific — inspect the actual DOM to confirm these selectors
        // var title = doc.DocumentNode
        //     .SelectSingleNode("//h1")?.InnerText.Trim() ?? string.Empty;

        // var author = doc.DocumentNode
        //     .SelectSingleNode("//*[contains(@class,'author')]")?.InnerText.Trim();

        // var dateStr = doc.DocumentNode
        //     .SelectSingleNode("//time[@datetime]")?.GetAttributeValue("datetime", null);

        // var contentNode = doc.DocumentNode
        //     .SelectSingleNode("//*[contains(@class,'article-body')] | //article");

        // var bodyText = contentNode is not null
        //     ? HtmlEntity.DeEntitize(contentNode.InnerText)
        //     : string.Empty;

        // // Normalize whitespace
        // bodyText = Regex.Replace(bodyText, @"\s+", " ").Trim();

        return new ParsedArticle
        {
           Url = "",
           Title = ""

        };
    }


}
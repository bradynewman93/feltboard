

namespace DesiringGodParser.Models;

public class ParsedArticle
{
    public required string Url {get; init;}

    public required string Title {get; init;}

    public string? Author {get; init;}

    public DateTime? PublishedDate {get; init;}

    public string BodyText {get; init;}

    public string? Key { get; set; }
}
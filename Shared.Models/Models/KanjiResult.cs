namespace Shared.Models.Models;

public class KanjiResult
{
    public char Kanji { get; set; }

    public string[] SelectedKunyomi { get; set; }

    public string[] SelectedOnyomi { get; set; }

    public string[] CorrectKunyomi { get; set; }

    public string[] CorrectOnyomi { get; set; }
}
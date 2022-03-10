namespace Benkyou.Domain.Models;

public class CardResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<KanjiResponse> KanjiList { get; set; }
}
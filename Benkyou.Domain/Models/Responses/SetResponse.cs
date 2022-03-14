namespace Benkyou.Domain.Models.Responses;

public class SetResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<KanjiResponse> KanjiList { get; set; }
}